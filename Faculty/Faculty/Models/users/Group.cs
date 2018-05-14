using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Faculty.Models.users
{
    public class Group
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Название группы")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Курс(Академический год)")]
        public byte Stage { get; set; }
        [Display(Name = "Група действующая")]
        public bool isActive { get; set; }
    }
}