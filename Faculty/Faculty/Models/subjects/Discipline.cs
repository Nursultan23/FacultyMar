using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Faculty.Models.subjects
{
    public class Discipline
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Название предмета")]
        public string Title { get; set; }
        [Required]
        [Display(Name = "Описание предмета")]
        public string Description { get; set; }
        [Display(Name = "Список литературы")]
        [DataType(DataType.MultilineText)]
        public string Literatures { get; set; }
        [Display(Name = "Действующая")]
        public bool IsActive { get; set; }
    }
}