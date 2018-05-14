using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DepartmentIntranet.Models
{
    public enum StudentStatus
    {
        /// <summary>
        /// учится
        /// </summary>
        Studying = 1,
        /// <summary>
        /// Отчислен
        /// </summary>
        Deducted = 2,
        /// <summary>
        /// академический отпуск
        /// </summary>
        AcademicHolidays = 3,
        /// <summary>
        /// закончил университет
        /// </summary>
        graduated = 4
    }
}