using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Collaboration.Models;
using System.Collections.Generic;
using System.IO;

namespace Collaboration.Controllers
{


    [Authorize]
    public class ManageController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        private ApplicationDbContext db = new ApplicationDbContext();

        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
        // GET: /Manage/Index
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Password was changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Password was set."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Настроен поставщик двухфакторной проверки подлинности."
                : message == ManageMessageId.Error ? "Error occured."
                : message == ManageMessageId.AddPhoneSuccess ? "Phone number is added."
                : message == ManageMessageId.RemovePhoneSuccess ? "Phone number is deleted."
                : message == ManageMessageId.ProfileUpdatedSuccess ? "Profile successfully updated."
                : "";

            var userId = User.Identity.GetUserId();
            ViewBag.CurrentUser = db.Users.Find(userId);
            ViewBag.Specifications = db.Specifications.ToList();
            var model = new IndexViewModel
            {
                HasPassword = HasPassword(),
                PhoneNumber = await UserManager.GetPhoneNumberAsync(userId),
                TwoFactor = await UserManager.GetTwoFactorEnabledAsync(userId),
                Logins = await UserManager.GetLoginsAsync(userId),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId)
            };
            return View(model);
        }

        public ActionResult Dashboard()
        {
            var user_id = User.Identity.GetUserId();
            ApplicationUser user = db.Users.Find(user_id);
            ViewBag.User = user;
            ViewBag.Specifications = db.Specifications.ToList();

            IEnumerable<EnrollmentRequests> er = db.EnrollmentRequests.Where(p => p.Project.PostedById == user_id).ToList();
            ViewBag.Requests = er;
            ViewBag.Count = er.Count();
            return View();
        }

        [HttpPost]
        public ActionResult Dashboard(string? project_title,
            string? project_description,
            int? specification_id,
            string? user_id,
            string? add_specification, int? specification_id_s)
        {
            if (add_specification != null)
            {
                var user_id_s = User.Identity.GetUserId();
                ApplicationUser user1 = db.Users.Find(user_id_s);
                user1.Specification = db.Specifications.Find(specification_id_s);
                db.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            Projects project = new Projects();
            project.Title = project_title;
            project.Description = project_description;
            project.SpecificationId = specification_id;
            project.PostedById = user_id;
            project.PostedDate = DateTime.Now;

            if (string.IsNullOrEmpty(project.Title))
            {
                ModelState.AddModelError("Title", "Title isn't set");
            }
            else if (project.Title.Length < 5)
            {
                ModelState.AddModelError("Title", "Title should contain at least 6 characters");
            }
            if (string.IsNullOrEmpty(project.Description))
            {
                ModelState.AddModelError("Description", "Description isn't set");
            }
            else if (project.Description.Length < 39)
            {
                ModelState.AddModelError("Description", "Description should contain at least 40 characters");
            }
            if (ModelState.IsValid)
            {
                ViewBag.Message = "Valid";
                db.Projects.Add(project);
                db.SaveChanges();
            }
            ViewBag.Message = "Non Valid";
            ApplicationUser user = db.Users.Find(user_id);
            ViewBag.User = user;
            ViewBag.Specifications = db.Specifications.ToList();

            IEnumerable<EnrollmentRequests> er = db.EnrollmentRequests.Where(p => p.Project.PostedById == user_id).ToList();
            ViewBag.Requests = er;
            ViewBag.Count = er.Count();
            return View(project);

        }

        [HttpPost]
        public ActionResult EditAvatar(string? user_id, HttpPostedFileBase uploadImage)
        {
            if (uploadImage != null)
            {
                byte[] imageData = null;
                using(var binaryReader = new BinaryReader(uploadImage.InputStream))
                {
                    imageData = binaryReader.ReadBytes(uploadImage.ContentLength);
                }
                ApplicationUser user = db.Users.Find(user_id);
                user.Image = imageData;

                db.SaveChanges();
            }
            return Redirect("/Manage/Dashboard");
        }

        [HttpPost]
        public ActionResult AddWork(string? user_id, string? work_title, string? work_description)
        {
            ApplicationUser user = db.Users.Find(user_id);
            Works work = new Works();
            work.Title = work_title;
            work.description = work_description;
            user.Works.Add(work);
            db.SaveChanges();
            return Redirect("/Manage/Dashboard");
        }

        [HttpPost]
        public ActionResult DeleteWork(int? work_id)
        {
            Works work = db.Works.Find(work_id);
            db.Works.Remove(work);
            db.SaveChanges();
            return Redirect("/Manage/Dashboard");
        }

        [HttpPost]
        public ActionResult ProjectDetails(string? cTitle, string? project_title,
                                          int? project_id)
        {
            Projects project = db.Projects.Find(project_id);
            if (cTitle != null)
            {
                
                if (string.IsNullOrEmpty(project_title))
                {
                    ModelState.AddModelError("Title", "Title isn't set");
                }
                else if (project_title.Length < 5)
                {
                    ModelState.AddModelError("Title", "Title should contain at least 6 characters");
                }
            }

            if (ModelState.IsValid)
            {
                if (project_title != null)
                {
                    project.Title = project_title;
                }
                db.SaveChanges();
            }
            var user_id = User.Identity.GetUserId();
            ApplicationUser CurrentUser = db.Users.Find(user_id);
            ViewBag.Countries = db.Countries.ToList();
            ViewBag.CurrentUser = CurrentUser;

            return View(project);
        }
        


        [HttpGet]
        public ActionResult ProjectDetails(int? project_id)
        {
            Projects project = db.Projects.Find(project_id);
            var user_id = User.Identity.GetUserId();
            ApplicationUser CurrentUser = db.Users.Find(user_id);
            ViewBag.Countries = db.Countries.ToList();
            ViewBag.CurrentUser = CurrentUser;
            return View(project);
        }

        [HttpPost]
        public ActionResult RemoveProject(int? project_id)
        {
            Projects project = db.Projects.Find(project_id);
            if (db.Enrollments.Where(p => p.ProjectId == project_id).SingleOrDefault() != null)
            {
                db.Enrollments.Remove(db.Enrollments.Where(p => p.ProjectId == project_id).SingleOrDefault());
            }
            if (db.EnrollmentRequests.Where(p => p.ProjectId == project_id).SingleOrDefault() != null)
            {
                db.EnrollmentRequests.Remove(db.EnrollmentRequests.Where(p => p.ProjectId == project_id).SingleOrDefault());
            }
                db.Projects.Remove(project);
            db.SaveChanges();
            return Redirect("/Manage/Dashboard");
        }

        [HttpPost]
        public ActionResult AssignCountry(int? project_id, int? country_id)
        {
            db.Projects.Find(project_id).Countries.Add(db.Countries.Find(country_id));
            db.Countries.Find(country_id).Projects.Add(db.Projects.Find(project_id));
            db.SaveChanges();
            return Redirect("/Manage/ProjectDetails?project_id=" + project_id);
        }

        [HttpPost]
        public ActionResult UnsignCountry(int? project_id, int? country_id)
        {
            db.Projects.Find(project_id).Countries.Remove(db.Countries.Find(country_id));
            db.Countries.Find(country_id).Projects.Remove(db.Projects.Find(project_id));
            db.SaveChanges();
            return Redirect("/Manage/ProjectDetails?project_id=" + project_id);
        }

        [HttpPost]
        public ActionResult RemoveMember(int? enrollment_id, int? project_id)
        {
            Enrollments enrollment = db.Enrollments.Find(enrollment_id);
            db.Enrollments.Remove(enrollment);
            db.SaveChanges();
            var user_id = User.Identity.GetUserId();
            Projects project = db.Projects.Find(project_id);
            if (!user_id.Equals(project.PostedById))
            {
                return Redirect("/Home/ProjectDetails?project_id=" + project_id);
            }
            return Redirect("/Manage/ProjectDetails?project_id=" + project_id);
        }

        [HttpPost]
        public ActionResult QuitProject(int? project_id, string? user_id)
        {
            Enrollments en = db.Enrollments.Where(p => p.ProjectId == project_id && p.UserId == user_id).SingleOrDefault();
            db.Enrollments.Remove(en);
            db.SaveChanges();
            return Redirect("/Home/Index");
        }
        //
        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessageId? message;
            var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("ManageLogins", new { Message = message });
        }

        //
        // GET: /Manage/
       
        public ActionResult AddPhoneNumber()
        {
            return View();
        }

        //
        // POST: /Manage/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Создание и отправка маркера
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), model.Number);
            if (UserManager.SmsService != null)
            {
                var message = new IdentityMessage
                {
                    Destination = model.Number,
                    Body = "Ваш код безопасности: " + code
                };
                await UserManager.SmsService.SendAsync(message);
            }
            return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number });
        }

        //
        // POST: /Manage/EnableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // POST: /Manage/DisableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // GET: /Manage/VerifyPhoneNumber
        public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), phoneNumber);
            // Отправка SMS через поставщик SMS для проверки номера телефона
            return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        }

        //
        // POST: /Manage/VerifyPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePhoneNumberAsync(User.Identity.GetUserId(), model.PhoneNumber, model.Code);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.AddPhoneSuccess });
            }
            // Это сообщение означает наличие ошибки; повторное отображение формы
            ModelState.AddModelError("", "Не удалось проверить телефон");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProfileUpdate(string? name,string? surname, int? specification_id)
        {
            var user_id = User.Identity.GetUserId();
            ApplicationUser user = db.Users.Find(user_id);
            user.Name = name;
            user.Surname = surname;
            user.SpecificationId = specification_id;
            if (ModelState.IsValid)
            {
                db.SaveChanges();
                return RedirectToAction("Index", new { Message = ManageMessageId.ProfileUpdatedSuccess });
            }

            return RedirectToAction("Index", new { Message = ManageMessageId.Error });
        }

        //
        // POST: /Manage/RemovePhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemovePhoneNumber()
        {
            var result = await UserManager.SetPhoneNumberAsync(User.Identity.GetUserId(), null);
            if (!result.Succeeded)
            {
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
        }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        //
        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // Это сообщение означает наличие ошибки; повторное отображение формы
            return View(model);
        }

        //
        // GET: /Manage/ManageLogins
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.RemoveLoginSuccess ? "Внешнее имя входа удалено."
                : message == ManageMessageId.Error ? "Произошла ошибка."
                : "";
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
            ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        //
        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Запрос перенаправления к внешнему поставщику входа для связывания имени входа текущего пользователя
            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
        }

        //
        // GET: /Manage/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
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

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error,
            ProfileUpdatedSuccess
        }

#endregion
    }
}