using Faculty.Models;
using Faculty.Models.subjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Faculty.Controllers
{
    public class NotificationController : Controller
    {
        public ActionResult Notifications()
        {
            ApplicationDbContext dbContext = new ApplicationDbContext();
            return View(dbContext.Notifications.ToList());
        }

        public ActionResult HeadNotifications()
        {
            ApplicationDbContext dbContext = new ApplicationDbContext();
            return View(dbContext.Notifications.ToList());
        }


        [Authorize]
        [HttpGet]
        public ActionResult AddNotification()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddNotification(Notification model)
        {
            
            ApplicationDbContext dbContext = new ApplicationDbContext();
            dbContext.Notifications.Add(model);
            dbContext.SaveChanges();
            return RedirectToAction("HeadNotifications");
        }

        public ActionResult DeleteNotification(int id)
        {
            ApplicationDbContext dbContext = new ApplicationDbContext();
            var _notification = dbContext.Notifications.FirstOrDefault(x => x.Id == id);
            dbContext.Notifications.Remove(_notification);
            dbContext.SaveChanges();
            return RedirectToAction("Notifications");
        }
    }
}