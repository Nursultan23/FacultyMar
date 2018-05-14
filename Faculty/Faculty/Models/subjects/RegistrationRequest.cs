using DepWeb.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Faculty.Models.subjects
{
    public class RegistrationRequest
    {

        public int Id { get; set; }
        [Required]
        public string Firstname { get; set; }
        [Required]
        public string MiddleName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string TelephoneNumber { get; set; }
        public string Password { get; set; }
        public RequestStatus status { get; set; }
        [Required]
        public DateTime AdditionalTime
        {
            get; set;
        }
        public int GroupId { get; set; }
    }
}