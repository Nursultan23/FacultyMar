using DepWeb.Models;
using Faculty.Models.users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Faculty.Models.ViewModels
{
    public class CourceSubscriptionViewModel
    {
        public int SubId { get; set; }
        public Student student { get; set; }
        public CourceViewModel cource { get; set; }

        public RequestStatus status { get; set; }


    }
}