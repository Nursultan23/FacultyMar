using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Faculty.Models.subjects
{
    public class Notification
    {
        public int Id { get; set; }
        [Display(Name = "Заголовок")]
        [Required]
        public string Title { get; set; }
        [Display(Name = "Текст")]
        [Required]
        [DataType(DataType.MultilineText)]
        public string Body { get; set; }
        [Display(Name = "Включен")]
        [Required]

        public bool Enable { get; set; }
    }
}