using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Faculty.Models.subjects
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int DisciplineId { get; set; }
        public bool isActive { get; set; }
        public byte Stage { get; set; }
        public bool IsElective { get; set; }
  
    }
    public class StudentCource
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int CourceId { get; set; }
    }

    public class Seminar
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public byte CreditCount { get; set; }
        public int ProcessorId { get; set; }
        public string Time { get; set; }
    }

    public class Lecture
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public byte CreditCount { get; set; }
        public int ProcessorId { get; set; }
        public string Time { get; set; }
    }
}