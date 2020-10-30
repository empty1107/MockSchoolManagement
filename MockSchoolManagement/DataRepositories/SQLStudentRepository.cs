using MockSchoolManagement.Infrastructure;
using MockSchoolManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MockSchoolManagement.DataRepositories
{
    public class SQLStudentRepository : IStudentRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<SQLStudentRepository> logger;

        public SQLStudentRepository(AppDbContext context, ILogger<SQLStudentRepository> logger)
        {
            this._context = context;
            this.logger = logger;
        }
        public Student Delete(int id)
        {
            Student student = _context.Students.Find(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                _context.SaveChanges();
            }
            return student;
        }

        public IEnumerable<Student> GetAllStudents()
        {
            logger.LogTrace("学生信息Trace(跟踪)Log");
            logger.LogDebug("学生信息Debug(调试)Log");
            logger.LogInformation("学生信息 信息(Information)Log");
            logger.LogWarning("学生信息 警告(Warning)Log");
            logger.LogError("学生信息 错误(Error)Log");
            logger.LogCritical("学生信息 严重(Critical)Log");

            return _context.Students;
        }

        public Student GetStudentById(int id)
        {
            return _context.Students.Find(id);
        }

        public Student Insert(Student student)
        {
            _context.Students.Add(student);
            _context.SaveChanges();
            return student;
        }

        public Student Update(Student updateStudent)
        {
            var student = _context.Students.Attach(updateStudent);
            student.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            return updateStudent;
        }
    }
}
