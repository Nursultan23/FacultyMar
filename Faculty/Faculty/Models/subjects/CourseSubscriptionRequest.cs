using DepWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Faculty.Models.subjects
{
    public class CourseSubscriptionRequest
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public RequestStatus status { get; set; }
    }
}