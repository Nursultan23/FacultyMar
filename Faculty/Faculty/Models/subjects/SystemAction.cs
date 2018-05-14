using DepartmentIntranet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Faculty.Models.subjects
{
    public class SystemAction
    {
        public int Id { get; set; }
        public DateTime Time { get; set; }
        public String Description { get; set; }
        public int ActorId { get; set; }
        public int SubjectId { get; set; }
        public SubjectType Type { get; set; }
    }
}