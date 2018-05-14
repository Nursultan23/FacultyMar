using DepartmentIntranet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Faculty.Models.users
{
    public class Student
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string userId { get; set; }
        public string UserName { get; set; }
        public DateTime AddmissionTime { get; set; }
        public DateTime GraduationTime { get; set; }
        public StudentStatus Status { get; set; }
        public byte Stage { get; set; }
        public int GroupId { get; set; }
    }
}