using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace DepartmentIntranet.Models
{
    public enum WorkerStatus
    {
        /// <summary>
        /// работает
        /// </summary>
        [Description("Работает")]
        working = 1,
        /// <summary>
        /// уволен
        /// </summary>
        [Description("Уволен")]
        dismissed = 2,
        /// <summary>
        /// в отпуске
        /// </summary>
        [Description("B отпуске")]
        onHoliday = 3,
        /// <summary>
        /// на больничном
        /// </summary>
        [Description("На больничном")]
        aid = 4
    }
}