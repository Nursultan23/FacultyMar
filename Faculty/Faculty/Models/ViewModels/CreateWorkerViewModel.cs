using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Faculty.Models.ViewModels
{
    public class CreateWorkerViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-mail")]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Номер телефона")]
        public string TelephoneNumber { get; set; }
        [Required]
        [Display(Name = "Имя")]
        public string Firstname { get; set; }
        [Required]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }
        [Display(Name = "Отчество")]
        public string MiddleName { get; set; }

        [Required]
        [Display(Name = "Степень/ученая степень")]
        public string ScienceDegree { get; set; }
        [Required]
        [Display(Name = "Статус")]
        public string Status { get; set; }
        [Display(Name = "Это преподаватель")]
        public bool isProfessor { get; set; }
   
        public List<SelectListItem> DropdownData { get; set; }
    }
}