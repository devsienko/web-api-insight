using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using WebApiMonitor.Administrator.Models;

namespace WebApiMonitor.Administrator.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private UserManager _userManager;
        
        public UserManager UserManager
        {
            get
            {
                return _userManager ?? new UserManager();
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
            var model = new IndexViewModel { };
            return View(model);
        }

        [HttpPost]
        public JsonResult Delete(string[] ids)
        {
            try
            {
                var userManager = new UserManager();
                userManager.RemoveByIds(ids);
                return Json(new
                {
                    msg = "Готово"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    msg = "Непредвиденная ошибка сервера. Пожалуйста попробутей позже."
                });
            }
        }

        //
        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            var message = string.Empty;
            //var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            //if (result.Succeeded)
            //{
            //    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            //    if (user != null)
            //    {
            //        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            //    }
            //    message = ManageMessageId.RemoveLoginSuccess;
            //}
            //else
            //{
            //    message = ManageMessageId.Error;
            //}
            return RedirectToAction("ManageLogins", new { Message = message });
        }

        //
        // GET: /Manage/AddPhoneNumber
        public ActionResult AddPhoneNumber()
        {
            return View();
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
            throw new NotImplementedException();
            //if (!ModelState.IsValid)
            //{
            //    return View(model);
            //}
            //var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            //if (result.Succeeded)
            //{
            //    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            //    if (user != null)
            //    {
            //        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            //    }
            //    return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            //}
            //AddErrors(result);
            //return View(model);
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
            throw new NotImplementedException();
            //if (ModelState.IsValid)
            //{
            //    var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
            //    if (result.Succeeded)
            //    {
            //        var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            //        if (user != null)
            //        {
            //            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            //        }
            //        return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
            //    }
            //    AddErrors(result);
            //}

            //// If we got this far, something failed, redisplay form
            //return View(model);
        }

        //
        // GET: /Manage/ManageLogins
        public async Task<ActionResult> Users(ManageMessageId? message)
        {
            var users = UserManager.GetUsers();
            return View(new UsersViewModel
            {
                Users = users
            });
        }

        //
        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
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

#region Helpers
        // Used for XSRF protection when adding external logins
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
            //var user = UserManager.FindByNameAsync(User.Identity.Name);
            //if (user != null)
            //{
            //    return user.PasswordHash != null;
            //}
            return false;
        }
        
        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

#endregion
    }
}