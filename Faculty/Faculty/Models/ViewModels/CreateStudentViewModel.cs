using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Faculty.Models.ViewModels
{
    public class CreateStudentViewModel
    {

        [Required]
        [Display(Name = "Имя")]
        public string Firstname { get; set; }
        [Required]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }
        [Display(Name = "Отчество")]
        public string MiddleName { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "Адрес электронной почты")]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Номер телефона")]
        public string TelephoneNumber { get; set; }
        [Required]
        [Display(Name = "Год поступления")]
        public string AdditionalYear { get; set; }
        [Required]
        [Display(Name ="Ваша группа")]
        public string GroupId { get; set; }
        public List<SelectListItem> GroupDropdownData { get; set; }

        public CreateStudentViewModel()
        {
            this.GroupDropdownData = new List<SelectListItem>();
        }

    }
}