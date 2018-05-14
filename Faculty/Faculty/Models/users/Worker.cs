using DepartmentIntranet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Faculty.Models.users
{
    public class Worker
    {
        public int id { get; set; }
        public string Firstname { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string ScienceDegree { get; set; }
        public WorkerStatus Status { get; set; }
        public bool isItProfessor { get; set; }
    }
}