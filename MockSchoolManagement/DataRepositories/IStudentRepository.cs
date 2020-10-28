﻿using MockSchoolManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.DataRepositories
{
    /// <summary>
    /// 学生仓储接口类
    /// </summary>
    public interface IStudentRepository
    {
        /// <summary>
        /// 通过ID获取学生信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Student GetStudentById(int id);
        /// <summary>
        /// 获取所有学生信息
        /// </summary>
        /// <returns></returns>
        IEnumerable<Student> GetAllStudents();
        /// <summary>
        /// 添加学生信息
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        Student Insert(Student student);
        ///  <summary>
        ///  修改学生信息
        ///  </summary>
        ///  <param name="updateStudent"> </param>
        ///  <returns> </returns>
        Student Update(Student updateStudent);
        ///  <summary>
        ///  删除学生信息
        ///  </summary>
        ///  <param name="id"> </param>
        ///  <returns> </returns>
        Student Delete(int id);
    }
}
