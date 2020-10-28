using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MockSchoolManagement.DataRepositories;
using MockSchoolManagement.Models;
using MockSchoolManagement.ViewModels;

namespace MockSchoolManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(IStudentRepository studentRepository, IWebHostEnvironment webHostEnvironment)
        {
            this._studentRepository = studentRepository;
            this._webHostEnvironment = webHostEnvironment;
        }

        public ViewResult Index()
        {
            //查询所有学生
            var model = _studentRepository.GetAllStudents();
            //将学生列表传递到视图
            return View(model);
        }

        public ViewResult Details(int id)
        {
            throw new Exception("在Details视图中抛出异常");
            Student student = _studentRepository.GetStudentById(id);
            if (student == null)
            {
                Response.StatusCode = 404;
                return View("StudentNotFound", id);
            }
            //实例化HomeDetailsViewModel并存储Student详细信息和PageTitle
            HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel
            {
                PageTitle = "学生详情",
                Student = student
            };
            //将viewModel对象传递给view()方法
            return View(homeDetailsViewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

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
                    //文件名保存在Student对象的PhotoPath属性中
                    //它将被保存到数据库Students表
                    PhotoPath = uniqueFileName
                };
                _studentRepository.Insert(newStudent);
                return RedirectToAction("Details", new { id = newStudent.Id });
            }
            return View();
        }

        [HttpGet]
        public ViewResult Edit(int id)
        {
            Student student = _studentRepository.GetStudentById(id);
            if (student == null)
            {
                Response.StatusCode = 404;
                return View("StudentNotFound",id);
            }
            StudentEditViewModel studentEditViewModel = new StudentEditViewModel
            {
                Id = student.Id,
                Name = student.Name,
                Email = student.Email,
                ExistingPhotoPath = student.PhotoPath,
                Major = student.Major,
            };
            return View(studentEditViewModel);
        }

        [HttpPost]
        public IActionResult Edit(StudentEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Student student = _studentRepository.GetStudentById(model.Id);
                //用模型中的数据，更新student对象
                student.Name = model.Name;
                student.Major = model.Major;
                student.Email = model.Email;

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
    }
}