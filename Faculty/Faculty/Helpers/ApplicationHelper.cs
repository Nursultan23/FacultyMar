using DepartmentIntranet.Models;
using DepWeb.Models;
using Faculty.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DepWeb.Helpers
{
    public static class ApplicationHelper
    {
        public static string GetWorkerStatusDesc(WorkerStatus status)
        {
            string _desc;
            switch (status)
            {
                case WorkerStatus.aid:
                    _desc = "На больничном";
                    break;
                case WorkerStatus.dismissed:
                    _desc = "Уволен";
                    break;
                case WorkerStatus.onHoliday:
                    _desc = "В отпуске";
                    break;
                case WorkerStatus.working:
                    _desc = "На работе";
                    break;
                default:
                    _desc = "не известно";
                    break;
            }
            return _desc;
        }

        public static string GetStudentStatusDesc(StudentStatus status)
        {
            string _desc;
            switch (status)
            {
                case StudentStatus.Studying:
                    _desc = "Учится";
                    break;
                case StudentStatus.graduated:
                    _desc = "Закончил университет";
                    break;
                case StudentStatus.Deducted:
                    _desc = "Отчислен";
                    break;
                case StudentStatus.AcademicHolidays:
                    _desc = "Академический отпуск";
                    break;
                default:
                    _desc = "не известно";
                    break;
            }
            return _desc;
        }

        public static string GeneratePassword(int length)
        {

            return "Aa=123456";
        }

        public static int GetStage(DateTime time)
        {
            int year = time.Year;
            int currYear = DateTime.Now.Year;
            if (currYear >= year)
            {
                int monthCount = DateTime.Now.Month + (currYear - year) * 12 - time.Month;
                return (int)(monthCount / 12) + 1;
            }
            else
            {
                return -1;
            }
        }

        public static void UpdateRegRequestStatus(int id, RequestStatus status)
        {
            ApplicationDbContext dbContext = new ApplicationDbContext();
            var _req = dbContext.RegistrationRequests.FirstOrDefault(x => x.Id == id);
            _req.status = status;
            dbContext.RegistrationRequests.Attach(_req);
            var entry = dbContext.Entry(_req);
            entry.Property(x => x.Id).IsModified = false;
            entry.Property(x => x.Firstname).IsModified = false;
            entry.Property(x => x.MiddleName).IsModified = false;
            entry.Property(x => x.LastName).IsModified = false;
            entry.Property(x => x.status).IsModified = true;
            entry.Property(x => x.TelephoneNumber).IsModified = false;
            entry.Property(x => x.Email).IsModified = false;
            dbContext.SaveChanges();
        }

        public static void UpdateSubRequestStatus(int id, RequestStatus status)
        {
            ApplicationDbContext dbContext = new ApplicationDbContext();
            var _req = dbContext.CourseSubscriptionRequests.FirstOrDefault(x => x.Id == id);
            _req.status = status;
            dbContext.CourseSubscriptionRequests.Attach(_req);
            var entry = dbContext.Entry(_req);
            entry.Property(x => x.Id).IsModified = false;
            entry.Property(x => x.StudentId).IsModified = false;
            entry.Property(x => x.CourseId).IsModified = false;
            entry.Property(x => x.status).IsModified = true;
            dbContext.SaveChanges();
        }

        public static int GetStudentCreditCount(int studentId)
        {
            ApplicationDbContext dbContext = new ApplicationDbContext();
            var studentGroupId = dbContext.Students.FirstOrDefault(x => x.Id == studentId).GroupId;
            var studentCourses = dbContext.StudentCources.Where(x => x.StudentId == studentId).ToList();
            int count = 0;
            if (studentCourses.Count() != 0 && studentCourses!=null)
            {
                foreach(var item in studentCourses)
                {
                    var _lection = dbContext.Lectures.FirstOrDefault(x => x.CourseId == item.CourceId);
                    var _seminar = dbContext.Seminars.FirstOrDefault(x => x.CourseId == item.CourceId);
                    count += _lection.CreditCount;
                    count += _seminar.CreditCount;
                }
            }

            //var notElectCourses = dbContext.Courses.Where(x => x.GroupId == studentGroupId).ToList();
            //if (notElectCourses.Count != 0)
            //{
            //    foreach(var item in notElectCourses)
            //    {
            //        var _lection = dbContext.Lectures.FirstOrDefault(x => x.CourseId == item.Id);
            //        var _seminar = dbContext.Seminars.FirstOrDefault(x => x.CourseId == item.Id);
            //        count += _lection.CreditCount;
            //        count += _seminar.CreditCount;
            //    }
            //}
            return count;
        }

        public static int GetStudentCourseCount(int studentId)
        {
            ApplicationDbContext dbContext = new ApplicationDbContext();
            var studentGroupId = dbContext.Students.FirstOrDefault(x => x.Id == studentId).GroupId;
            int count = 0;
            count+= dbContext.StudentCources.Where(x => x.StudentId == studentId).Count();
            //count += dbContext.Courses.Where(x => x.GroupId == studentGroupId).Count();
            return count;
        }

        public static int GetProfessorCreditCount(int ProfessorId)
        {
            ApplicationDbContext dbContext = new ApplicationDbContext();

            var _lection = dbContext.Lectures.Where(x => x.ProcessorId == ProfessorId).ToList();
            var _seminar = dbContext.Seminars.Where(x => x.ProcessorId == ProfessorId).ToList();

            int count = 0;

            if (_lection.Count()!=0)
            {
                foreach (var item in _lection)
                {
                    count += item.CreditCount;
                }
            }
            if (_seminar.Count() != 0)
            {
                foreach (var item in _seminar)
                {
                    count += item.CreditCount;
                }
            }
            return count;
        }

        public static int GetProfessorCourseCount(int ProfessorId)
        {
            ApplicationDbContext dbContext = new ApplicationDbContext();
            int count = 0;
            count += dbContext.Lectures.Where(x => x.ProcessorId == ProfessorId).Count();
            count += dbContext.Seminars.Where(x => x.ProcessorId == ProfessorId).Count();
            return count;
        }
    }
}