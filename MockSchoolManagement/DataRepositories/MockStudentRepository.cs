using MockSchoolManagement.Models;
using MockSchoolManagement.Models.EnumTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.DataRepositories
{
    /// <summary>
    /// 仓储服务
    /// </summary>
    public class MockStudentRepository : IStudentRepository
    {
        private List<Student> _studentList;
        public MockStudentRepository()
        {
            _studentList = new List<Student>(){new Student() {Id = 1,Name = "张三",Major =  MajorEnum.ComputerScience,Email ="zhangsan@52abp.com" },
                                               new Student() {Id = 2,Name = "李四",Major =MajorEnum.Mathematics,Email ="lisi@52abp.com" },
                                               new Student() {Id = 3,Name = "赵六",Major = MajorEnum.ElectronicCommerce,Email ="zhaoliu@52abp.com" }};
        }

        public Student Delete(int id)
        {
            Student student = _studentList.FirstOrDefault(s => s.Id == id);
            if (student !=null)
            {
                _studentList.Remove(student);
            }
            return student;
        }

        /// <summary>
        /// 获取所有学生
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Student> GetAllStudents()
        {
            return _studentList;
        }

        /// <summary>
        /// 根据ID获取学生信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Student GetStudentById(int id)
        {
            return _studentList.FirstOrDefault(a => a.Id == id);
        }

        public Student Insert(Student student)
        {
            student.Id = _studentList.Max(s => s.Id) + 1;
            _studentList.Add(student);
            return student;
        }

        public Student Update(Student updateStudent)
        {
            Student student = _studentList.FirstOrDefault(x => x.Id == updateStudent.Id);
            if (student!=null)
            {
                student.Name = updateStudent.Name;
                student.Email = updateStudent.Email;
                student.Major = updateStudent.Major;
            }
            return student;
        }
    }
}
