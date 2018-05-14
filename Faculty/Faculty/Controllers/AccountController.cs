using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Faculty.Models;
using Faculty.Models.ViewModels;
using DepartmentIntranet.Models;
using DepWeb.Helpers;
using System.Collections.Generic;
using Faculty.Models.users;
using Faculty.Models.subjects;
using DepWeb.Models;

namespace Faculty.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;


        #region Standart methods
        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

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

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Сбои при входе не приводят к блокированию учетной записи
            // Чтобы ошибки при вводе пароля инициировали блокирование учетной записи, замените на shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Неудачная попытка входа.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Требовать предварительный вход пользователя с помощью имени пользователя и пароля или внешнего имени входа
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Приведенный ниже код защищает от атак методом подбора, направленных на двухфакторные коды. 
            // Если пользователь введет неправильные коды за указанное время, его учетная запись 
            // будет заблокирована на заданный период. 
            // Параметры блокирования учетных записей можно настроить в IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Неправильный код.");
                    return View(model);
            }
        }
        
        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Не показывать, что пользователь не существует или не подтвержден
                    return View("ForgotPasswordConfirmation");
                }

                // Дополнительные сведения о включении подтверждения учетной записи и сброса пароля см. на странице https://go.microsoft.com/fwlink/?LinkID=320771.
                // Отправка сообщения электронной почты с этой ссылкой
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Сброс пароля", "Сбросьте ваш пароль, щелкнув <a href=\"" + callbackUrl + "\">здесь</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // Появление этого сообщения означает наличие ошибки; повторное отображение формы
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Не показывать, что пользователь не существует
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Запрос перенаправления к внешнему поставщику входа
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Создание и отправка маркера
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Выполнение входа пользователя посредством данного внешнего поставщика входа, если у пользователя уже есть имя входа
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // Если у пользователя нет учетной записи, то ему предлагается создать ее
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Получение сведений о пользователе от внешнего поставщика входа
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Вспомогательные приложения
        // Используется для защиты от XSRF-атак при добавлении внешних имен входа
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion

        #endregion

        [AllowAnonymous]
        public ActionResult Register()
        {
            RegisterViewModel model = new RegisterViewModel();
            ApplicationDbContext dbContext = new ApplicationDbContext();
            List<Group> listGroup = new List<Group>();
            listGroup = dbContext.Groups.Where(x => x.isActive).ToList();
            if (listGroup.Count == 0)
            {
                ViewBag.Success = false;
                ViewBag.Message = "Вы не можете создать студента, так как группы еще не добавили";
                return View("Result");
            }
            foreach (var item in listGroup)
            {
                model.GroupDropdownData.Add(new SelectListItem { Text = item.Name + "(" + item.Stage + "курс)", Value = item.Id.ToString() });
            }

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public  ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationDbContext dbContext = new ApplicationDbContext();
                List<Group> listGroup = new List<Group>();
                listGroup = dbContext.Groups.Where(x => x.isActive).ToList();
                foreach (var item in listGroup)
                {
                    model.GroupDropdownData.Add(new SelectListItem { Text = item.Name + "(" + item.Stage + "курс)", Value = item.Id.ToString() });
                }

                
                var _existReq = dbContext.RegistrationRequests.FirstOrDefault(x => x.Email == model.Email);
                if (_existReq != null)
                {
                    ViewBag.Success = false;
                    ViewBag.Message = "Такой пользовтель уже существует";
                    return View("Result");
                }
                int additionalYear;

                if (!int.TryParse(model.AdditionalYear, out additionalYear))
                {
                    ModelState.AddModelError("", "Год не правильно введен!");
                    return View(model);
                }
                var additionalTime = new DateTime(additionalYear, 9, 1);

                int stage = ApplicationHelper.GetStage(additionalTime);
                if (stage < 0 || stage > 4)
                {
                    ViewBag.Success = false;
                    ViewBag.Message = "Не правильные данные года поступления";
                    return View("Result");
                }
                int _grid;
                if (int.TryParse(model.GroupId, out _grid) && stage != (dbContext.Groups.FirstOrDefault(x => x.Id == _grid)).Stage)
                {
                    ModelState.AddModelError("", "Вы не можете присоединится в данную группу, так как курсы не совпадают!");
                    return View(model);
                }

                try
                {
                    if (int.TryParse(model.AdditionalYear, out additionalYear))
                    {
                        RegistrationRequest request = new RegistrationRequest
                        {
                            Firstname = model.Firstname,
                            MiddleName = model.MiddleName ?? "",
                            LastName = model.LastName,
                            Email = model.Email,
                            TelephoneNumber = model.TelephoneNumber,
                            Password = "",
                            status = RequestStatus.Active,
                            GroupId = int.Parse(model.GroupId),
                            AdditionalTime = new DateTime(additionalYear, 9, 1)
                        };
                        dbContext.RegistrationRequests.Add(request);
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("", "Год не правильно введен!");
                        return View(model);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Заявка прошла не удачно!");

                }

            }
            ViewBag.Success = true;
            ViewBag.Message = "Ваша заявка на регистрацию отправлена. Подойдите к сотруднику кафедры для получения учетных данных";
            // Появление этого сообщения означает наличие ошибки; повторное отображение формы
            return View("Result");
        }

        [HttpGet]
        [Authorize(Roles ="dephead")]
        public ActionResult CreateAdmin()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult CreateFacultyHead()
        {
            CreateFacultyHeadViewModel model = new CreateFacultyHeadViewModel();
            model.DropdownData = new List<SelectListItem>();
            foreach (WorkerStatus item in Enum.GetValues(typeof(WorkerStatus)))
            {
                model.DropdownData.Add(new SelectListItem
                {
                    Text = ApplicationHelper.GetWorkerStatusDesc(item),
                    Value = ((int)item).ToString()
                });
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult CreateFacultyHead(CreateFacultyHeadViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser _tempUser=new ApplicationUser();
                try
                {
                    ApplicationDbContext dbContext = new ApplicationDbContext();
                    var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                    string password = ApplicationHelper.GeneratePassword(5);
                    var result = UserManager.Create(user, password);
                    if (result.Succeeded)
                    {
                        _tempUser = user;
                        UserManager.AddToRole(user.Id, "dephead");
                        Worker worker = new Worker();
                        worker.Firstname = model.Firstname;
                        worker.MiddleName = model.MiddleName != null ? model.MiddleName : "";
                        worker.LastName = model.LastName;
                        worker.UserId = user.Id;
                        worker.ScienceDegree = model.ScienceDegree;
                        worker.Status = (WorkerStatus)int.Parse(model.Status);
                        worker.isItProfessor = false;

                        dbContext.Workers.Add(worker);
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
                catch (Exception)
                {
                   var  _res = UserManager.Delete(_tempUser);
                    AddErrors(_res);
                }

                model.DropdownData = new List<SelectListItem>();
                foreach (WorkerStatus item in Enum.GetValues(typeof(WorkerStatus)))
                {
                    model.DropdownData.Add(new SelectListItem
                    {
                        Text = ApplicationHelper.GetWorkerStatusDesc(item),
                        Value = ((int)item).ToString()
                    });
                }
            }

            return View(model);
        }



        [Authorize]
        [HttpGet]
        public ActionResult CreateWorker()
        {
            if (GetMyRoles()!=null || GetMyRoles().Contains("admin") || GetMyRoles().Contains("dephead"))
            {

                CreateWorkerViewModel model = new CreateWorkerViewModel();
                model.DropdownData = new List<SelectListItem>();
                foreach (WorkerStatus item in Enum.GetValues(typeof(WorkerStatus)))
                {
                    model.DropdownData.Add(new SelectListItem
                    {
                        Text = ApplicationHelper.GetWorkerStatusDesc(item),
                        Value = ((int)item).ToString()
                    });
                }

                return View(model);
            }
            else
            {
                ViewBag.Success = false;
                ViewBag.Message = "У вас нет доступа";
                return View("Result");
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult CreateWorker(CreateWorkerViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationDbContext dbContext = new ApplicationDbContext();
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                string password = ApplicationHelper.GeneratePassword(5);

                var result = UserManager.Create(user, password);

                if (result.Succeeded)
                {


                    if (model.isProfessor)
                    {
                        UserManager.AddToRole(user.Id, "professor");
                    }
                    else
                    {
                        UserManager.AddToRole(user.Id, "Personal");
                    }
                    try
                    {
                        Worker worker = new Worker();
                        worker.Firstname = model.Firstname;
                        worker.MiddleName = model.MiddleName != null ? model.MiddleName : "";
                        worker.LastName = model.LastName;
                        worker.UserId = user.Id;
                        worker.ScienceDegree = model.ScienceDegree;
                        worker.Status = (WorkerStatus)int.Parse(model.Status);
                        worker.isItProfessor = model.isProfessor;
                        worker.UserName = user.UserName;
                        dbContext.Workers.Add(worker);
                        dbContext.SaveChanges();

                        ViewBag.Success = true;
                        ViewBag.Message = "Студент:" + worker.LastName + " " + worker.Firstname + " Логин:" + user.Email + " Пароль:" + password;
                        return View("Result");
                    }
                    catch (Exception)
                    {
                        var _result = UserManager.Delete(user);
                        ViewBag.Success = false;
                        ViewBag.Message = "Добавление не удалось";
                        return View("Result");
                    }
                    
                }
                else
                {
                    AddErrors(result);
                }
                model.DropdownData = new List<SelectListItem>();
                foreach (WorkerStatus item in Enum.GetValues(typeof(WorkerStatus)))
                {
                    model.DropdownData.Add(new SelectListItem
                    {
                        Text = ApplicationHelper.GetWorkerStatusDesc(item),
                        Value = ((int)item).ToString()
                    });
                }

            }

            return View(model);
        }


        [Authorize]
        [HttpGet]
        public ActionResult CreateSudent()
        {
            CreateStudentViewModel model = new CreateStudentViewModel();
            if (GetMyRoles() != null || GetMyRoles().Contains("admin") || GetMyRoles().Contains("dephead") || GetMyRoles().Contains("personal"))
            {
                ApplicationDbContext dbContext = new ApplicationDbContext();
                List<Group> listGroup = new List<Group>();
                listGroup = dbContext.Groups.Where(x=>x.isActive).ToList();
                if (listGroup.Count == 0)
                {
                    ViewBag.Success = false;
                    ViewBag.Message = "Вы не можете создать студента, так как группы еще не добавили";
                    return View("Result");
                }
                foreach (var item in listGroup)
                {
                    model.GroupDropdownData.Add(new SelectListItem { Text = item.Name + "(" + item.Stage + "курс)", Value = item.Id.ToString() });
                }
            }
            else
            {
                ViewBag.Success = false;
                ViewBag.Message = "У вас нет доступа";
                return View("Result");
            }
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult CreateSudent(CreateStudentViewModel model)
        {
            ApplicationDbContext dbContext = new ApplicationDbContext();
            if (ModelState.IsValid)
            {
                foreach (var item in dbContext.Groups.Where(x=>x.isActive).ToList())
                {
                    model.GroupDropdownData.Add(new SelectListItem { Text = item.Name+"("+item.Stage+ "курс)", Value = item.Id.ToString() });
                }
                int additionalYear;

                if (!int.TryParse(model.AdditionalYear, out additionalYear))
                {
                    ModelState.AddModelError("", "Год не правильно введен!");
                    return View(model);
                }
                var additionalTime = new DateTime(additionalYear, 9, 1);

                int stage = ApplicationHelper.GetStage(additionalTime);
                if (stage < 0 || stage > 4)
                {
                    ViewBag.Success = false;
                    ViewBag.Message = "Не правильные данные года поступления";
                    return View("Result");
                }
                int _grid;
                if (int.TryParse(model.GroupId,out _grid) && stage != (dbContext.Groups.FirstOrDefault(x => x.Id == _grid)).Stage)
                {
                    ModelState.AddModelError("", "Вы не можете добавить студента в данную группу, так как курсы не совпадают!");
                    return View(model);
                }

                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                string password = ApplicationHelper.GeneratePassword(5);

                var result = UserManager.Create(user, password);

                if (result.Succeeded)
                {
                    UserManager.AddToRole(user.Id, "student");
                   
                    try
                    {
                        Student student = new Student();
                        student.Firstname = model.Firstname;
                        student.MiddleName = model.MiddleName;
                        student.LastName = model.LastName;
                        student.userId = user.Id;
                        student.UserName = user.UserName;
                        student.AddmissionTime = additionalTime;
                        student.GraduationTime = additionalTime.AddMonths(45);
                        student.Status = StudentStatus.Studying;
                        student.Stage = (byte)stage;
                        student.GroupId = int.Parse(model.GroupId);

                        dbContext.Students.Add(student);
                        dbContext.SaveChanges();

                        ViewBag.Success = true;
                        ViewBag.Message = "Студент:" + student.LastName + " " + student.Firstname + " Логин:" + user.Email + " Пароль:" + password;
                        return View("Result");
                    }
                    catch (Exception ex)
                    {
                        var _result = UserManager.Delete(user);
                        ViewBag.Success = false;
                        ViewBag.Message = "Добавление не удалось";
                        return View("Result");
                    }

                }
                else
                {
                    AddErrors(result);
                }
            }

            return View(model);
        }

        [Authorize]
        [HttpGet]
        public ActionResult CreateGroup()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult CreateGroup(Group model)
        {
            ApplicationDbContext dbContext = new ApplicationDbContext();
            if (ModelState.IsValid)
            {
                try
                {
                    dbContext.Groups.Add(model);
                    dbContext.SaveChanges();
                }
                catch 
                {
                    ViewBag.Success = false;
                    ViewBag.Message = "Добавление не удалось";
                    return View("Result");
                }
               
            }
            return RedirectToAction("Groups","Home");
        }


        [Authorize]
        public ActionResult Registrate()
        {
            return View();
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

        public ActionResult DepartmentPersonal()
        {
            ApplicationDbContext dbContext = new ApplicationDbContext();
            List<Worker> model = dbContext.Workers.ToList();

            return View(model);
        }

        public ActionResult RegistrateStudent(int id)
        {
            ApplicationUser curruser = new ApplicationUser();
            try
            {

                ApplicationDbContext dbContext = new ApplicationDbContext();
                var _req = dbContext.RegistrationRequests.FirstOrDefault(x => x.Id == id);
                

                var user = new ApplicationUser { UserName = _req.Email, Email = _req.Email };
                string password = ApplicationHelper.GeneratePassword(5);
                var result = UserManager.Create(user, password);
                if (result.Succeeded)
                {
                    curruser = user;
                    UserManager.AddToRole(user.Id, "student");


                    int stage = ApplicationHelper.GetStage(_req.AdditionalTime);
                    if (stage < 0 || stage > 4)
                    {
                        if (curruser != null)
                        {
                            var _result = UserManager.Delete(curruser);
                        }
                        ViewBag.Success = false;
                        ViewBag.Message = "Не правильные данные года поступления";
                        return View("Result");
                    }
                    Student student = new Student
                    {
                        AddmissionTime = _req.AdditionalTime,
                        GraduationTime=_req.AdditionalTime.AddMonths(45),
                        Firstname = _req.Firstname,
                        MiddleName = _req.MiddleName,
                        LastName = _req.LastName,
                        userId = user.Id,
                        UserName = user.UserName,
                        Status = StudentStatus.Studying,
                        Stage = (byte)stage,
                        GroupId = _req.GroupId
                };
                    dbContext.Students.Add(student);
                    dbContext.SaveChanges();
                    ApplicationHelper.UpdateRegRequestStatus(_req.Id, RequestStatus.Confirmed);

                    ViewBag.Success = true;
                    ViewBag.Message = "Студент:" + student.LastName + student.Firstname + " Логин:" + user.Email + " Пароль:" + password;
                    return View("Result");

                }
            }
            catch (Exception)
            {
                if (curruser != null)
                {
                    var result = UserManager.Delete(curruser);
                }

                ViewBag.Success = false;
                ViewBag.Message = "Сохранение не удалось";
                return View("Result");
            }
            return View();
        }

        public ActionResult RejectRegStudent(int id)
        {
            ApplicationHelper.UpdateRegRequestStatus(id, RequestStatus.Rejected);
            return RedirectToAction("RegistrationRequests", "Home");
        }

        public ActionResult Students()
        {
            ApplicationDbContext dbContext = new ApplicationDbContext();
            List<Student> model = dbContext.Students.OrderBy(x=>x.Stage).ToList();

            return View(model);
        }
        [Authorize]
        public ActionResult Student(int id)
        {
            ApplicationDbContext dbContext = new ApplicationDbContext();
            return View(dbContext.Students.FirstOrDefault(x => x.Id == id));
        }

        [Authorize]
        public ActionResult Worker(int id)
        {
            ApplicationDbContext dbContext = new ApplicationDbContext();
            return View(dbContext.Workers.FirstOrDefault(x => x.id == id));
        }




        //[Authorize]
        //[HttpGet]
        //public ActionResult EditStudent(int id)
        //{
        //    ApplicationDbContext dbContext = new ApplicationDbContext();
        //    var student = dbContext.Students.FirstOrDefault(x => x.Id == id);
        //    CreateStudentViewModel model = new CreateStudentViewModel();
        //    model.Firstname = student.Firstname;
        //    model.LastName = student.LastName;
        //    model.MiddleName = student.MiddleName;
        //    model.Email = student.UserName ?? "";
        //    model.TelephoneNumber = "";
        //    model.AdditionalYear=
        //    return View();
        //}
        //[Authorize]

        //[HttpGet]
        //public ActionResult EditStudent(CreateStudentViewModel model)
        //{
        //    return View();
        //}


        //[Authorize]
        //[HttpGet]
        //public ActionResult EditWorker(int id)
        //{
        //    return View();
        //}
        //[Authorize]

        //[HttpGet]
        //public ActionResult EditWorker(CreateWorkerViewModel model)
        //{
        //    return View();
        //}

    }
}