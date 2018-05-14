using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Faculty.Models.ViewModels
{
    public class OfficeViewModel
    {
        public int Id { get; set; }
        public string Role { get; set; }
        public string Firstname { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string userId { get; set; }

        public DateTime AddmissionTime { get; set; }
        public DateTime GraduationTime { get; set; }
        public string StudentStatus { get; set; }

        public int CreditCount { get; set; }
        public int CourseCount { get; set; }


        public string ScienceDegree { get; set; }
        public string WorkerStatus { get; set; }
        public bool isItProfessor { get; set; }
        public int RegisterRequestCount { get; set; }
        public int CourseRequestCount { get; set; }
    }
}