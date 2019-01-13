using LexLibrary.Rbac.Areas.LexLibraryRbac.ViewModels;
using LexLibrary.Rbac.Attributes;
using LexLibrary.Rbac.Extensions;
using LexLibrary.Rbac.Models;
using LexLibrary.Rbac.Models.Consts;
using LexLibrary.Rbac.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text;

namespace LexLibrary.Rbac.Areas.LexLibraryRbac.Controllers
{
    [Area("LexLibraryRbac")]
    [AllowAnonymous]
    public class AccountController : Controller
    {
        #region Ctor

        private readonly LexLibraryRbacSetting _setting = null;
        private readonly IUserManager _userManage = null;

        public AccountController(
            LexLibraryRbacSetting setting,
            IUserManager userManage)
        {
            _setting = setting;
            _userManage = userManage;
        }

        #endregion

        #region 登入

        [LexLibraryRbacAuthorizedRedirect]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [LexLibraryRbacAuthorizedRedirect]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = _userManage.Login(model.Account, model.Password, model.IsPersistent);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.RtnMsg);
                return View(model);
            }

            return redirectToSafeUrl(model.ReturnUrl);
        }

        #endregion

        #region 登出

        public IActionResult Logout()
        {
            _userManage.Logout();
            return redirectToSafeUrl();
        }

        #endregion

        #region 註冊

        [LexLibraryRbacAuthorizedRedirect]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [LexLibraryRbacAuthorizedRedirect]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = _userManage.Register(model.Account, model.Password, model.Email, model.DisplayName);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.RtnMsg);
                return View(model);
            }

            return redirectAndAlert(nameof(Login), "註冊成功，已發送驗證信");
        }

        #endregion

        #region 忘記密碼

        [LexLibraryRbacAuthorizedRedirect]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [LexLibraryRbacAuthorizedRedirect]
        public IActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = _userManage.ForgotPassword(model.Account, model.Email);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.RtnMsg);
                return View(model);
            }

            return redirectAndAlert(Url.Action(nameof(Login)), result.RtnMsg);
        }

        #endregion

        #region 重設密碼

        [LexLibraryRbacAuthorizedRedirect]
        public IActionResult ResetPassword(string d)
        {
            if (!validResetPasswordSecret(d, out IActionResult errorResult, out long userId))
            {
                return errorResult;
            }

            return View(new ResetPasswordViewModel
            {
                Secret = d
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [LexLibraryRbacAuthorizedRedirect]
        public IActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!validResetPasswordSecret(model.Secret, out IActionResult errorResult, out long userId))
            {
                return errorResult;
            }

            var result = _userManage.ChangePassword(userId, model.Password);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.RtnMsg);
                return View(model);
            }

            return redirectAndAlert(Url.Action(nameof(Login)), result.RtnMsg);
        }

        #endregion

        #region 信箱驗證

        public IActionResult ConfirmEmail(string d)
        {
            var result = _userManage.ConfirmEmail(d);

            string baseUrl = Request.GetBaseUrl();
            return redirectAndAlert(baseUrl, result.RtnMsg);
        }

        #endregion

        #region Private Method

        private bool validResetPasswordSecret(string d, out IActionResult result, out long userId)
        {
            string baseUrl = Request.GetBaseUrl();

            var validResult = _userManage.ValidateToken(UserTokens.ResetPassword, d, out userId);
            result = redirectAndAlert(baseUrl, validResult.RtnMsg);

            return validResult.IsSuccess;
        }

        private IActionResult redirectToSafeUrl(string url = null)
        {
            string baseUrl = Request.GetBaseUrl();
            if (!string.IsNullOrWhiteSpace(url) &&
                (Url.IsLocalUrl(url) || url.StartsWith(baseUrl, StringComparison.OrdinalIgnoreCase)))
            {
                return Redirect(url);
            }
            else
            {
                return Redirect(baseUrl);
            }
        }

        private IActionResult redirectAndAlert(string url, string message)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<script>");
            sb.AppendFormat("window.alert('{0}');", message?.Replace("'", "\""));
            sb.AppendFormat("window.location.href = '{0}';", url?.Replace("'", "\""));
            sb.Append("</script>");

            return Content(sb.ToString(), "text/html", Encoding.UTF8);
        }

        #endregion
    }
}
