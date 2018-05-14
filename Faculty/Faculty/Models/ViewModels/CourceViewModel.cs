using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Faculty.Models.ViewModels
{
    public class CourceViewModel
    {
        [Display(Name = "Название курса")]
        public string Title { get; set; }
        [Display(Name = "Количество кредитов")]
        public int CreditCount { get; set; }
        [Display(Name = "Количество кред.(Лекция)")]
        public int lecCreditCount { get; set; }
        [Display(Name = "Количество кред.(Семинар)")]
        public int SemCreditCount { get; set; }
        [Display(Name = "Преподователь(лекция)")]
        public string lecProfessor { get; set; }
        [Display(Name = "Преподователь(Семинар)")]
        public string semProfessor { get; set; }
        [Display(Name = "Описание")]
        public string Description { get; set; }
        [Display(Name = "Список литератур")]
        public string Literatures { get; set; }
        public int CourseId { get; set; }
        public bool isElective { get; set; }
    }
}