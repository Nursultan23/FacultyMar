using DepartmentIntranet.Models;
using DepWeb.Helpers;
using DepWeb.Models;
using Faculty.Models;
using Faculty.Models.subjects;
using Faculty.Models.users;
using Faculty.Models.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Faculty.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ActionResult Index()
        {
            ApplicationDbContext dbContext = new ApplicationDbContext();
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(dbContext));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(dbContext));

            var roles = roleManager.Roles;
            var role1 = new IdentityRole { Name = "admin" };
            var role2 = new IdentityRole { Name = "dephead" };
            var role3 = new IdentityRole { Name = "personal" };
            var role4 = new IdentityRole { Name = "student" };
            var role5 = new IdentityRole { Name = "professor" };

            if (roles.Count() == 0)
            {

                roleManager.Create(role1);
                roleManager.Create(role2);
                roleManager.Create(role3);
                roleManager.Create(role4);
                roleManager.Create(role5);
            }
            var user = userManager.FindByName("admin@mail.ru");
            if (user == null)
            {
                var admin = new ApplicationUser { Email = "admin@mail.ru", UserName = "admin@mail.ru" };
                string password = "Aa=123456";
                var result = userManager.Create(admin, password);
                if (result.Succeeded)
                {
                    userManager.AddToRole(admin.Id, role1.Name);
                }
            }
            return View();
        }

        [Authorize]
        public ActionResult Office()
        {
            ApplicationDbContext dbContext = new ApplicationDbContext();
            OfficeViewModel model = new OfficeViewModel();
            IList<string> roles = new List<string>();
            ApplicationUserManager userManager = HttpContext.GetOwinContext()
                .GetUserManager<ApplicationUserManager>();
            ApplicationUser user = userManager.FindByEmail(User.Identity.Name);
            if (user != null)
            {
                roles = userManager.GetRoles(userId: user.Id);
            }

            if (roles.Count != 0)
            {
                switch (roles.First())
                {
                    case "admin":
                        model.Role = "admin";
                        model.userId = user.Id;
                        model.Email = user.Email;
                        break;
                    case "dephead":
                        model.Role = "dephead";
                        model.userId = user.Id;
                        model.Email = user.Email;
                        Worker dephead = dbContext.Workers.FirstOrDefault(x => x.UserId.ToString() == user.Id);
                        if (dephead != null)
                        {
                            model.Id = dephead.id;
                            model.Firstname = dephead.Firstname;
                            model.MiddleName = dephead.MiddleName;
                            model.LastName = dephead.LastName;
                            model.ScienceDegree = dephead.ScienceDegree;
                            model.WorkerStatus = ApplicationHelper.GetWorkerStatusDesc(dephead.Status);
                            model.isItProfessor = false;
                            model.RegisterRequestCount = dbContext.RegistrationRequests.Where(x => x.status == RequestStatus.Active).Count();
                            model.CourseRequestCount = dbContext.CourseSubscriptionRequests.Where(x => x.status == RequestStatus.Active).Count();
                        }
                        break;
                    case "personal":
                        model.Role = "personal";
                        model.userId = user.Id;
                        model.Email = user.Email;
                        Worker personal = dbContext.Workers.FirstOrDefault(x => x.UserId.ToString() == user.Id);
                        if (personal != null)
                        {
                            model.Id = personal.id;
                            model.Firstname = personal.Firstname;
                            model.MiddleName = personal.MiddleName;
                            model.LastName = personal.LastName;
                            model.ScienceDegree = personal.ScienceDegree;
                            model.WorkerStatus = ApplicationHelper.GetWorkerStatusDesc(personal.Status);
                            model.isItProfessor = false;
                            model.RegisterRequestCount = dbContext.RegistrationRequests.Where(x => x.status == RequestStatus.Active).Count();
                            model.CourseRequestCount = dbContext.CourseSubscriptionRequests.Where(x => x.status == RequestStatus.Active).Count();
                        }

                        break;
                    case "professor":
                        model.Role = "professor";
                        model.userId = user.Id;
                        model.Email = user.Email;
                        Worker professor = dbContext.Workers.FirstOrDefault(x => x.UserId.ToString() == user.Id);
                        if (professor != null)
                        {
                            model.Id = professor.id;
                            model.Firstname = professor.Firstname;
                            model.MiddleName = professor.MiddleName;
                            model.LastName = professor.LastName;
                            model.ScienceDegree = professor.ScienceDegree;
                            model.WorkerStatus = ApplicationHelper.GetWorkerStatusDesc(professor.Status);
                            model.isItProfessor = true;
                            model.CreditCount = ApplicationHelper.GetProfessorCreditCount(professor.id);
                            model.CourseCount = ApplicationHelper.GetProfessorCourseCount(professor.id);
                        }

                        break;
                    case "student":
                        model.Role = "student";
                        model.userId = user.Id;
                        model.Email = user.Email;
                        Student student = dbContext.Students.FirstOrDefault(x => x.userId.ToString() == user.Id);
                        if (student != null)
                        {
                            model.Id = student.Id;
                            model.Firstname = student.Firstname;
                            model.MiddleName = student.MiddleName;
                            model.LastName = student.LastName;
                            model.AddmissionTime = student.AddmissionTime;
                            model.GraduationTime = student.GraduationTime;
                            model.WorkerStatus = ApplicationHelper.GetStudentStatusDesc(student.Status);
                            model.isItProfessor = false;
                            model.CourseCount = ApplicationHelper.GetStudentCourseCount(student.Id);
                            model.CreditCount = ApplicationHelper.GetStudentCreditCount(student.Id);
                        }
                        break;
                }

            }
            else
            {
                
            }
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public ActionResult Groups()
        {
            ApplicationDbContext dbContext = new ApplicationDbContext();
            var grouplist = dbContext.Groups.ToList();
            return View(grouplist);
        }

        public ActionResult AddCourse()
        {
            ApplicationDbContext dbContext = new ApplicationDbContext();
            CourseEditViewModel model = new CourseEditViewModel();

            model.DropdownProfessor = new List<SelectListItem>();


            foreach (var item in dbContext.Workers.Where(x => x.isItProfessor && x.Status != WorkerStatus.dismissed))
            {
                model.DropdownProfessor.Add(new SelectListItem
                {
                    Text = item.LastName + item.Firstname + "(" + item.ScienceDegree + ")",
                    Value = item.id.ToString()
                });
            }
            model.DropdownDiscipline = new List<SelectListItem>();
            foreach (var item in dbContext.Disciplines.Where(x => x.IsActive))
            {
                model.DropdownDiscipline.Add(new SelectListItem
                {
                    Text = item.Title,
                    Value = item.Id.ToString()
                });
            }

            model.GroupDropdownData= new List<SelectListItem>();
            model.GroupDropdownData.Add(new SelectListItem
            {
                Text = "--Выберите группу--",
                Value = "0"
            });
            foreach (var item in dbContext.Groups.Where(x => x.isActive))
            {
                model.GroupDropdownData.Add(new SelectListItem
                {
                    Text = item.Name+"("+item.Stage+"-курс)",
                    Value = item.Id.ToString()
                });
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult AddCourse(CourseEditViewModel model)
        {
            Lecture _oldLecture = new Lecture();
            Seminar _oldseminar = new Seminar();
            Course _oldcourse = new Course();

            ApplicationDbContext dbContext = new ApplicationDbContext();

            //if(!model.isElective && model.GroupId==0){
            //    ModelState.AddModelError("", "Для не элективных курсов необходимо выбрать группу");
            //}
            //if (model.isElective && model.GroupId != 0)
            //{
            //    ModelState.AddModelError("", "Для элективных курсов не нужно выбирать группу");
            //}

            if (ModelState.IsValid)
            {
                try
                {

                    var cource = new Course
                    {
                        DisciplineId = int.Parse(model.DisciplineId),
                        isActive = model.isAlive,
                        IsElective = model.isElective,
                        //GroupId = model.GroupId
                    };
                    dbContext.Courses.Add(cource);
                    dbContext.SaveChanges();
                    _oldcourse = cource;

                    var seminar = new Seminar
                    {
                        ProcessorId = int.Parse(model.seminar_ProfessorID),
                        CreditCount = (byte)model.semCreditCount,
                        Time = model.seminarTime,
                        CourseId = cource.Id

                    };
                    dbContext.Seminars.Add(seminar);
                    dbContext.SaveChanges();
                    _oldseminar = seminar;

                    var lecture = new Lecture
                    {
                        ProcessorId = int.Parse(model.Lecture_seminar_ProfessorID),
                        CreditCount = (byte)model.lecCreditCount,
                        Time = model.lectureTime,
                        CourseId = cource.Id

                    };
                    dbContext.Lectures.Add(lecture);
                    dbContext.SaveChanges();
                    _oldLecture = lecture;



                    ViewBag.Success = true;
                    ViewBag.Message = "Сохранение прошло удачно";
                    return View("Result");
                }
                catch (Exception ex)
                {
                    if (_oldcourse != null)
                    {
                        dbContext.Courses.Remove(_oldcourse);
                        dbContext.SaveChanges();
                    }
                    if (_oldLecture != null)
                    {
                        dbContext.Lectures.Remove(_oldLecture);
                        dbContext.SaveChanges();
                    }
                    if (_oldseminar != null)
                    {
                        dbContext.Seminars.Remove(_oldseminar);
                        dbContext.SaveChanges();
                    }

                    ViewBag.Success = false;
                    ViewBag.Message = "Сохранение прошло не удачно.Обратитесь к админстратору";
                    return View("Result");
                }
            }
            else
            {
                model.DropdownProfessor = new List<SelectListItem>();
                foreach (var item in dbContext.Workers.Where(x => x.isItProfessor && x.Status != WorkerStatus.dismissed))
                {
                    model.DropdownProfessor.Add(new SelectListItem
                    {
                        Text = item.LastName + item.Firstname + "(" + item.ScienceDegree + ")",
                        Value = item.id.ToString()
                    });
                }
                model.DropdownDiscipline = new List<SelectListItem>();
                foreach (var item in dbContext.Disciplines.Where(x => x.IsActive))
                {
                    model.DropdownDiscipline.Add(new SelectListItem
                    {
                        Text = item.Title,
                        Value = item.Id.ToString()
                    });
                }

                model.GroupDropdownData = new List<SelectListItem>();
                model.GroupDropdownData.Add(new SelectListItem
                {
                    Text = "--Выберите группу--",
                    Value = "0"
                });
                foreach (var item in dbContext.Groups.Where(x => x.isActive))
                {
                    model.GroupDropdownData.Add(new SelectListItem
                    {
                        Text = item.Name + "(" + item.Stage + "-курс)",
                        Value = item.Id.ToString()
                    });
                }
                return View(model);
            }
          
        }

        [HttpGet]
        public ActionResult AddDiscipline()
        {
            return View();
        }
        [HttpPost]
        [Authorize]
        public ActionResult AddDiscipline(Discipline model)
        {
            ApplicationDbContext dbContext = new ApplicationDbContext();
            try
            {
                var _old = dbContext.Disciplines.FirstOrDefault(x => x.IsActive && x.Title == model.Title);
                if (_old != null)
                {
                    ViewBag.Success = false;
                    ViewBag.Message = "Предмет с таким именем уже существует";
                    return View("Result");
                }
                dbContext.Disciplines.Add(model);
                dbContext.SaveChanges();
                ViewBag.Success = true;
                ViewBag.Message = "Сохранение прошло удачно";
                return View("Result");
            }
            catch (Exception ex)
            {
                ViewBag.Success = false;
                ViewBag.Message = "Сохранение прошло не удачно.Обратитесь к админстратору";
                return View("Result");
            }
        }


        [Authorize]
        public ActionResult Courses()
        {
            ApplicationDbContext dbContext = new ApplicationDbContext();
            var courses = new List<CourceViewModel>();
            var _courses = dbContext.Courses.ToList();
            if (_courses.Count != 0)
            {
                try
                {
                    foreach (var item in _courses)
                    {
                        var _lecture = dbContext.Lectures.FirstOrDefault(x => x.CourseId == item.Id);
                        var _seminar = dbContext.Seminars.FirstOrDefault(x => x.CourseId == item.Id);
                        var _discipline = dbContext.Disciplines.FirstOrDefault(x => x.Id == item.DisciplineId);

                        var _lectureProf = dbContext.Workers.FirstOrDefault(x => x.id == _lecture.ProcessorId);
                        var _seminarProf = dbContext.Workers.FirstOrDefault(x => x.id == _seminar.ProcessorId);

                        var _course = new CourceViewModel
                        {
                            Title = _discipline.Title,
                            CreditCount = _seminar.CreditCount + _lecture.CreditCount,
                            lecCreditCount = _lecture.CreditCount,
                            SemCreditCount = _seminar.CreditCount,
                            lecProfessor = _lectureProf.LastName + " " + _lectureProf.Firstname + "(" + _lectureProf.ScienceDegree + ")",
                            semProfessor = _seminarProf.LastName + " " + _seminarProf.Firstname + "(" + _seminarProf.ScienceDegree + ")",
                            Description = _discipline.Description,
                            Literatures = _discipline.Literatures,
                            CourseId = item.Id,
                            isElective=item.IsElective
                        };
                        courses.Add(_course);
                    }
                    return View(courses);
                }
                catch (Exception ex)
                {
                    ViewBag.Success = false;
                    ViewBag.Message = "Произашла ошибка при загрузке данных";
                    return View("Result");
                }

            }
            else
            {
                ViewBag.Success = false;
                ViewBag.Message = "Созданных курсов нет. Добавьте чтобы посмотреть";
                return View("Result");
            }

        }
        [Authorize]
        public ActionResult Course(int id)
        {
            ApplicationDbContext dbContext = new ApplicationDbContext();
            var _coursedb = dbContext.Courses.FirstOrDefault(x => x.Id == id);
            if (_coursedb != null)
            {
                try
                {
                    var _lecture = dbContext.Lectures.FirstOrDefault(x => x.CourseId == _coursedb.Id);
                    var _seminar = dbContext.Seminars.FirstOrDefault(x => x.CourseId == _coursedb.Id);
                    var _discipline = dbContext.Disciplines.FirstOrDefault(x => x.Id == _coursedb.DisciplineId);

                    var _lectureProf = dbContext.Workers.FirstOrDefault(x => x.id == _lecture.ProcessorId);
                    var _seminarProf = dbContext.Workers.FirstOrDefault(x => x.id == _lecture.ProcessorId);

                    var _course = new CourceViewModel
                    {
                        Title = _discipline.Title,
                        CreditCount = _seminar.CreditCount + _lecture.CreditCount,
                        lecCreditCount = _lecture.CreditCount,
                        SemCreditCount = _seminar.CreditCount,
                        lecProfessor = _lectureProf.LastName + " " + _lectureProf.Firstname + "(" + _lectureProf.ScienceDegree + ")",
                        semProfessor = _seminarProf.LastName + " " + _seminarProf.Firstname + "(" + _seminarProf.ScienceDegree + ")",
                        Description = _discipline.Description,
                        Literatures = _discipline.Literatures,
                        CourseId = _coursedb.Id
                    };

                    return View(_course);
                }
                catch (Exception)
                {
                    ViewBag.Success = false;
                    ViewBag.Message = "Произашла ошибка при загрузке данных";
                    return View("Result");
                }

            }
            else
            {
                ViewBag.Success = false;
                ViewBag.Message = "Произашла ошибка при загрузке данных";
                return View("Result");
            }

        }

        [Authorize]
        public ActionResult RegistrationRequests()
        {
            if (GetMyRoles() != null || GetMyRoles().Contains("admin") || GetMyRoles().Contains("dephead") || GetMyRoles().Contains("personal"))
            {
                ApplicationDbContext dbContext = new ApplicationDbContext();
                List<RegistrationRequest> requests = dbContext.RegistrationRequests.ToList();
                return View(requests);
            }
            else
            {
                ViewBag.Success = false;
                ViewBag.Message = "У вас нет доступа";
                return View("Result");
            }
           
        }
        [Authorize]
        public ActionResult RegistrationRequest(int id)
        {
            if (GetMyRoles() != null || GetMyRoles().Contains("admin") || GetMyRoles().Contains("dephead") || GetMyRoles().Contains("personal"))
            {
                ApplicationDbContext dbContext = new ApplicationDbContext();
                RegistrationRequest request = dbContext.RegistrationRequests.FirstOrDefault(x => x.Id == id);
                return View(request);
            }
            else
            {
                ViewBag.Success = false;
                ViewBag.Message = "У вас нет доступа";
                return View("Result");
            }
        }

        public ActionResult CreateCourseReq()
        {
            return View();

        }

        public ActionResult ElectiveCourses()
        {
            ApplicationDbContext dbContext = new ApplicationDbContext();
            var courses = new List<CourceViewModel>();
            var _courses = dbContext.Courses.ToList();
            if (_courses.Count != 0)
            {
                try
                {
                    foreach (var item in _courses)
                    {
                        var _lecture = dbContext.Lectures.FirstOrDefault(x => x.CourseId == item.Id);
                        var _seminar = dbContext.Seminars.FirstOrDefault(x => x.CourseId == item.Id);
                        var _discipline = dbContext.Disciplines.FirstOrDefault(x => x.Id == item.DisciplineId);

                        var _lectureProf = dbContext.Workers.FirstOrDefault(x => x.id == _lecture.ProcessorId);
                        var _seminarProf = dbContext.Workers.FirstOrDefault(x => x.id == _seminar.ProcessorId);

                        var _course = new CourceViewModel
                        {
                            Title = _discipline.Title,
                            CreditCount = _seminar.CreditCount + _lecture.CreditCount,
                            lecCreditCount = _lecture.CreditCount,
                            SemCreditCount = _seminar.CreditCount,
                            lecProfessor = _lectureProf.LastName + " " + _lectureProf.Firstname + "(" + _lectureProf.ScienceDegree + ")",
                            semProfessor = _seminarProf.LastName + " " + _seminarProf.Firstname + "(" + _seminarProf.ScienceDegree + ")",
                            Description = _discipline.Description,
                            Literatures = _discipline.Literatures,
                            CourseId = item.Id
                        };
                        courses.Add(_course);
                    }
                    return View(courses);
                }
                catch (Exception)
                {
                    ViewBag.Success = false;
                    ViewBag.Message = "Произашла ошибка при загрузке данных";
                    return View("Result");
                }

            }
            else
            {
                ViewBag.Success = false;
                ViewBag.Message = "Созданных курсов нет. Добавьте чтобы посмотреть";
                return View("Result");
            }
        }

        private List<string> GetMyRoles()
        {
            ApplicationUser user = UserManager.FindByEmail(User.Identity.Name);
            if (user != null)
            {
                return UserManager.GetRoles(user.Id).ToList();
            }
            else
            {
                return null;
            }
        }

        public ActionResult StudentCourse(int id)
        {

            ApplicationDbContext dbContext = new ApplicationDbContext();
            var _coursedb = dbContext.Courses.FirstOrDefault(x => x.Id == id);

            if (_coursedb != null)
            {
                try
                {
                    var _lecture = dbContext.Lectures.FirstOrDefault(x => x.CourseId == _coursedb.Id);
                    var _seminar = dbContext.Seminars.FirstOrDefault(x => x.CourseId == _coursedb.Id);
                    var _discipline = dbContext.Disciplines.FirstOrDefault(x => x.Id == _coursedb.DisciplineId);

                    var _lectureProf = dbContext.Workers.FirstOrDefault(x => x.id == _lecture.ProcessorId);
                    var _seminarProf = dbContext.Workers.FirstOrDefault(x => x.id == _seminar.ProcessorId);

                    var _course = new CourceViewModel
                    {
                        Title = _discipline.Title,
                        CreditCount = _seminar.CreditCount + _lecture.CreditCount,
                        lecCreditCount = _lecture.CreditCount,
                        SemCreditCount = _seminar.CreditCount,
                        lecProfessor = _lectureProf.LastName + " " + _lectureProf.Firstname + "(" + _lectureProf.ScienceDegree + ")",
                        semProfessor = _seminarProf.LastName + " " + _seminarProf.Firstname + "(" + _seminarProf.ScienceDegree + ")",
                        Description = _discipline.Description,
                        Literatures = _discipline.Literatures,
                        CourseId = _coursedb.Id
                    };

                    return View(_course);
                }
                catch (Exception)
                {
                    ViewBag.Success = false;
                    ViewBag.Message = "Произашла ошибка при загрузке данных";
                    return View("Result");
                }

            }
            else
            {
                ViewBag.Success = false;
                ViewBag.Message = "Произашла ошибка при загрузке данных";
                return View("Result");
            }
        }

        [Authorize]
        public ActionResult CourseRequest(int id)
        {
            if (GetMyRoles() != null || GetMyRoles().Contains("Student"))
            {
                try
                {
                    ApplicationUser user = UserManager.FindByEmail(User.Identity.Name);
                    ApplicationDbContext dbContext = new ApplicationDbContext();
                    CourseSubscriptionRequest request = new CourseSubscriptionRequest();
                    request.CourseId = id;
                    request.status = RequestStatus.Active;
                    request.StudentId = dbContext.Students.FirstOrDefault(x => x.userId == user.Id).Id;
                    dbContext.CourseSubscriptionRequests.Add(request);
                    dbContext.SaveChanges();

                    ViewBag.Success = true;
                    ViewBag.Message = "Заявка была отправлена успешно";
                    return View("Result");
                }
                catch
                {
                    ViewBag.Success = false;
                    ViewBag.Message = "Пройзашла ошибка при отправке запроса.Попробуйте позже";
                    return View("Result");
                }

                
            }
            else
            {
                ViewBag.Success = false;
                ViewBag.Message = "У вас нет доступа";
                return View("Result");
            }
        }

        public ActionResult CourseSubscriptionRequests()
        {
            ApplicationDbContext dbContext = new ApplicationDbContext();
            List<CourceSubscriptionViewModel> model = new List<CourceSubscriptionViewModel>();

            var subs = dbContext.CourseSubscriptionRequests.ToList();
            if(subs.Count!=0 && subs != null)
            {
                foreach (var item in subs)
                {
                    Student _student = dbContext.Students.FirstOrDefault(x => x.Id == item.StudentId);
                    CourceViewModel _courceViewModel = GetCourseModel(item.CourseId);
                    model.Add(new CourceSubscriptionViewModel
                    {
                        student = _student,
                        cource = _courceViewModel,
                        status = item.status,
                        SubId=item.Id
                    });
                }
                
            }
            return View(model);
            

        }

        public ActionResult SubscriptionRequest(int id)
        {
            ApplicationDbContext dbContext = new ApplicationDbContext();
            CourceSubscriptionViewModel  model = new CourceSubscriptionViewModel();
            var sub = dbContext.CourseSubscriptionRequests.FirstOrDefault(x => x.Id == id);
            Student _student = dbContext.Students.FirstOrDefault(x => x.Id == sub.StudentId);
            CourceViewModel _courceViewModel = GetCourseModel(id);

            model.student = _student;
            model.cource = _courceViewModel;
            model.status = sub.status;
            model.SubId = sub.Id;
            return View(model);
        }

        public ActionResult ConnfirmSubscription(int id)
        {
            ApplicationDbContext dbContext = new ApplicationDbContext();
            var sub = dbContext.CourseSubscriptionRequests.FirstOrDefault(x => x.Id == id);
            try
            {
                StudentCource studentCource = new StudentCource
                {
                    StudentId = sub.StudentId,
                    CourceId=sub.CourseId
                };
                dbContext.StudentCources.Add(studentCource);
                dbContext.SaveChanges();
                ApplicationHelper.UpdateSubRequestStatus(id, RequestStatus.Confirmed);
                ViewBag.Success = true;
                ViewBag.Message = "Сохранение прошло удачно";
                return View("Result");
            }
            catch
            {
                ApplicationHelper.UpdateSubRequestStatus(id, RequestStatus.Active);
                ViewBag.Success = false;
                ViewBag.Message = "Сохранение не удалось";
                return View("Result");
            }

        }

        public ActionResult RejectSubscription(int id)
        {
            ApplicationHelper.UpdateSubRequestStatus(id, RequestStatus.Rejected);
            ViewBag.Success = true;
            ViewBag.Message = "Удачно";
            return View("Result");
        }
        
        private CourceViewModel GetCourseModel(int courseId)
        {
            ApplicationDbContext dbContext = new ApplicationDbContext();
            var _coursedb = dbContext.Courses.FirstOrDefault(x => x.Id == courseId);
            if (_coursedb != null)
            {

                var _lecture = dbContext.Lectures.FirstOrDefault(x => x.CourseId == _coursedb.Id);
                var _seminar = dbContext.Seminars.FirstOrDefault(x => x.CourseId == _coursedb.Id);
                var _discipline = dbContext.Disciplines.FirstOrDefault(x => x.Id == _coursedb.DisciplineId);

                var _lectureProf = dbContext.Workers.FirstOrDefault(x => x.id == _lecture.ProcessorId);
                var _seminarProf = dbContext.Workers.FirstOrDefault(x => x.id == _lecture.ProcessorId);

                var _course = new CourceViewModel
                {
                    Title = _discipline.Title,
                    CreditCount = _seminar.CreditCount + _lecture.CreditCount,
                    lecCreditCount = _lecture.CreditCount,
                    SemCreditCount = _seminar.CreditCount,
                    lecProfessor = _lectureProf.LastName + " " + _lectureProf.Firstname + "(" + _lectureProf.ScienceDegree + ")",
                    semProfessor = _seminarProf.LastName + " " + _seminarProf.Firstname + "(" + _seminarProf.ScienceDegree + ")",
                    Description = _discipline.Description,
                    Literatures = _discipline.Literatures,
                    CourseId = _coursedb.Id
                };
                return _course;
            }
            else
            {
                return null;
            }


    
        }

        [Authorize]
        public ActionResult MyCourses(int id)
        {
            ApplicationDbContext dbContext = new ApplicationDbContext();
            var courses = new List<CourceViewModel>();
            var studentCourses = dbContext.StudentCources.Where(x => x.StudentId == id).ToList() ;
            var _courses = new List<Course>();
            if (studentCourses.Count!=0 && studentCourses != null)
            {
                foreach(var item in studentCourses)
                {
                    _courses.Add(dbContext.Courses.FirstOrDefault(x => x.Id == item.CourceId));
                }
            }
           
            if (_courses.Count != 0)
            {
                try
                {
                    foreach (var item in _courses)
                    {
                        var _lecture = dbContext.Lectures.FirstOrDefault(x => x.CourseId == item.Id);
                        var _seminar = dbContext.Seminars.FirstOrDefault(x => x.CourseId == item.Id);
                        var _discipline = dbContext.Disciplines.FirstOrDefault(x => x.Id == item.DisciplineId);

                        var _lectureProf = dbContext.Workers.FirstOrDefault(x => x.id == _lecture.ProcessorId);
                        var _seminarProf = dbContext.Workers.FirstOrDefault(x => x.id == _lecture.ProcessorId);

                        var _course = new CourceViewModel
                        {
                            Title = _discipline.Title,
                            CreditCount = _seminar.CreditCount + _lecture.CreditCount,
                            lecCreditCount = _lecture.CreditCount,
                            SemCreditCount = _seminar.CreditCount,
                            lecProfessor = _lectureProf.LastName + " " + _lectureProf.Firstname + "(" + _lectureProf.ScienceDegree + ")",
                            semProfessor = _seminarProf.LastName + " " + _seminarProf.Firstname + "(" + _seminarProf.ScienceDegree + ")",
                            Description = _discipline.Description,
                            Literatures = _discipline.Literatures,
                            CourseId = item.Id,
                            isElective = item.IsElective
                        };
                        courses.Add(_course);
                    }
                    return View(courses);
                }
                catch (Exception)
                {
                    ViewBag.Success = false;
                    ViewBag.Message = "Произашла ошибка при загрузке данных";
                    return View("Result");
                }

            }
            else
            {
                ViewBag.Success = false;
                ViewBag.Message = "Созданных курсов нет. Добавьте чтобы посмотреть";
                return View("Result");
            }

        }

      
    }
}