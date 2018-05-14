using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Faculty.Models.ViewModels
{
    public class CourseEditViewModel
    {
        [Display(Name = "Предмет")]
        public string DisciplineId { get; set; }
        public List<SelectListItem> DropdownDiscipline { get; set; }


        public string SeminarId { get; set; }
        [Display(Name = "Преподователь семинаров")]
        public string seminar_ProfessorID { get; set; }
        [Display(Name = "Время уроков")]
        public string seminarTime { get; set; }
        [Display(Name = "Количество кредитов")]
        public int semCreditCount { get; set; }


        public string Lectureid { get; set; }
        [Display(Name = "Лектор")]
        public string Lecture_seminar_ProfessorID { get; set; }
        [Display(Name = "Время уроков")]
        public string lectureTime { get; set; }
        [Display(Name = "Количество кредитов")]
        public int lecCreditCount { get; set; }


        public List<SelectListItem> DropdownProfessor { get; set; }
        [Display(Name = "Курс действующий")]
        public bool isAlive { get; set; }

        [Display(Name = "Курс элективный")]
        public bool isElective { get; set; }
        [Display(Name ="Группа")]
        public int GroupId { get; set; }
        public List<SelectListItem> GroupDropdownData { get; set; }
    }
}