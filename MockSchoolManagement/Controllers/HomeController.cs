using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MockSchoolManagement.Infrastructure.Repositories;
using MockSchoolManagement.Models;
using MockSchoolManagement.Security.CustomTokenProvider;
using MockSchoolManagement.ViewModels;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using MockSchoolManagement.Application.Dtos;
using MockSchoolManagement.Application.Students;

namespace MockSchoolManagement.Controllers
{
    /// <summary>
    /// 主页控制器
    /// </summary>
    public class HomeController : Controller
    {
        private readonly IRepository<Student, int> _studentRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<HomeController> logger;
        private readonly IStudentService _studentService;
        private readonly IDataProtector _protector;

        public HomeController(IRepository<Student, int> studentRepository, IWebHostEnvironment webHostEnvironment, ILogger<HomeController> logger, IDataProtectionProvider dataProtectionProvider, DataProtectionPurposeStrings dataProtectionPurposeStrings, IStudentService studentService)
        {
            this._studentRepository = studentRepository;
            this._webHostEnvironment = webHostEnvironment;
            this.logger = logger;
            this._studentService = studentService;
            this._protector = dataProtectionProvider.CreateProtector(dataProtectionPurposeStrings.StudentIdRouteValue);
        }

        public async Task<IActionResult> Index(string searchString, int currentPage = 1, string sortBy = "Id")
        {
            ViewBag.CurrentFilter = searchString?.Trim();
            PaginationModel paginationModel = new PaginationModel();
            //总记录数
            paginationModel.Count = await _studentRepository.CountAsync();
            //当前页
            paginationModel.CurrentPage = currentPage;
            //获取分页结果
            var students = await _studentService.GetPaginatedResult(paginationModel.CurrentPage, searchString, sortBy);
            paginationModel.Data = students.Select(s =>
            {
                s.EncryptedId = _protector.Protect(s.Id.ToString());
                return s;
            }).ToList();
            //将学生列表传递到视图
            return View(paginationModel);
        }

        public ViewResult Details(string id)
        {
            Student student = DecryptedStudent(id);
            if (student == null)
            {
                ViewBag.ErrorMessage = $"学生Id={id}的信息不存在，请重试。";
                return View("NotFound", id);
            }
            //实例化HomeDetailsViewModel并存储Student详细信息和PageTitle
            HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel
            {
                PageTitle = "学生详情",
                Student = student
            };
            homeDetailsViewModel.Student.EncryptedId = _protector.Protect(student.Id.ToString());
            //将viewModel对象传递给view()方法
            return View(homeDetailsViewModel);
        }

        /// <summary>
        /// 解密学生信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private Student DecryptedStudent(string id)
        {
            //使用Unprotect()来解析StudentID
            string decryptedId = _protector.Unprotect(id);
            int decryptedStudentId = Convert.ToInt32(decryptedId);
            Student student = _studentRepository.FirstOrDefault(s => s.Id == decryptedStudentId);
            return student;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// 新增学生
        /// </summary>
        /// <param name="model">学生实体</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Create(StudentCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;
                if (model.Photos != null && model.Photos.Count > 0)
                {
                    foreach (IFormFile photo in model.Photos)
                    {
                        //必须将图片文件上传到wwwroot的images文件夹
                        //而要获取wwwroot文件夹路径，需要注入Asp.net Core提供的WebHostEnvironment服务
                        string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "avatars");
                        if (!Directory.Exists(uploadFolder))
                        {
                            Directory.CreateDirectory(uploadFolder);
                        }
                        //为了确保文件名是唯一的，我们在文件名后附加一个新的GUID值和一个下划线
                        uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
                        string filePath = Path.Combine(uploadFolder, uniqueFileName);
                        //使用IFormFile接口提供的copyTo()方法将文件复制到wwwroot/images
                        photo.CopyTo(new FileStream(filePath, FileMode.Create));
                    }
                }
                Student newStudent = new Student
                {
                    Name = model.Name,
                    Email = model.Email,
                    Major = model.Major,
                    EnrollmentDate = model.EnrollmentDate,
                    //文件名保存在Student对象的PhotoPath属性中
                    //它将被保存到数据库Students表
                    PhotoPath = uniqueFileName
                };
                _studentRepository.Insert(newStudent);
                var encryptedId = _protector.Protect(newStudent.Id.ToString());
                return RedirectToAction("Details", new { id = encryptedId });
            }
            return View();
        }

        [HttpGet]
        public ViewResult Edit(string id)
        {
            Student student = DecryptedStudent(id);
            if (student == null)
            {
                ViewBag.ErrorMessage = $"学生Id={id}的信息不存在，请重试。";
                return View("NotFound", id);
            }
            StudentEditViewModel studentEditViewModel = new StudentEditViewModel
            {
                Id = student.Id,
                Name = student.Name,
                Email = student.Email,
                ExistingPhotoPath = student.PhotoPath,
                Major = student.Major,
                EnrollmentDate = student.EnrollmentDate
            };
            return View(studentEditViewModel);
        }

        [HttpPost]
        public IActionResult Edit(StudentEditViewModel model)
        {
            //检查提供的数据是否有效，如果没有验证通过，则需要重新编辑学生信息
            if (ModelState.IsValid)
            {
                Student student = DecryptedStudent(model.Id.ToString());
                //用模型中的数据，更新student对象
                student.Name = model.Name;
                student.Major = model.Major;
                student.Email = model.Email;
                student.EnrollmentDate = model.EnrollmentDate;

                //如果修改了头像，需要上传。没有修改则会保留之前的头像。
                if (model.Photos.Count > 0)
                {
                    if (model.ExistingPhotoPath != null)//删除旧头像
                    {
                        string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "avatars", model.ExistingPhotoPath);
                        System.IO.File.Delete(filePath);
                    }
                    //保存新头像
                    student.PhotoPath = ProcessUploadedFile(model);
                }
                //更新数据库
                Student updateStudent = _studentRepository.Update(student);
                //跳转主页
                return RedirectToAction("Index");
            }
            return View(model);
        }

        /// <summary>
        /// 将图片保存至 wwwroot/avatars，并返回唯一文件名
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private string ProcessUploadedFile(StudentEditViewModel model)
        {
            string uniqueFileName = null;
            if (model.Photos.Count > 0)
            {
                foreach (IFormFile photo in model.Photos)
                {
                    //必须将图片文件上传到wwwroot的images文件夹
                    //而要获取wwwroot文件夹路径，需要注入Asp.net Core提供的WebHostEnvironment服务
                    string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "avatars");
                    if (!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                    }
                    //为了确保文件名是唯一的，我们在文件名后附加一个新的GUID值和一个下划线
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
                    string filePath = Path.Combine(uploadFolder, uniqueFileName);

                    //使用了非托管资源，所以需要手动释放资源
                    //使用IFormFile接口提供的copyTo()方法将文件复制到wwwroot/images
                    using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        photo.CopyTo(fileStream);
                    }
                }
            }
            return uniqueFileName;
        }

        /// <summary>
        /// 删除学生
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var student = await _studentRepository.FirstOrDefaultAsync(a => a.Id == id);
            if (student == null)
            {
                ViewBag.ErrorMessage = $"无法找到ID为{id}的学生信息";
                return View("NotFound");
            }
            await _studentRepository.DeleteAsync(a => a.Id == id);
            return RedirectToAction("Index");
        }
    }
}