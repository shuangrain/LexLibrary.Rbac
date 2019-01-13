using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace LexLibrary.Rbac.Models
{
    public class LexLibraryRbacSetting
    {
        public string CookieName { get; set; } = ".LexLibrary.Rbac";

        public string WebSiteName { get; set; } = "LexLibrary.Rbac";

        public PathString LoginPath { get; set; } = "/LexLibraryRbac/Account/Login";

        public PathString LogoutPath { get; set; } = "/LexLibraryRbac/Account/Logout";

        public PathString ResetPasswordPath { get; set; } = "/LexLibraryRbac/Account/ResetPassword";

        public PathString ConfirmEmailPath { get; set; } = "/LexLibraryRbac/Account/ConfirmEmail";

        public EmailTemplate ResetPasswordTemplate { get; set; } = new EmailTemplate
        {
            Title = "[{0}] 重新設定您的密碼",
            Body = "<a href='{0}' target='_blank'>點我</a>重設您的密碼。",
            IsHtml = true
        };

        public EmailTemplate ConfirmEmailTemplate { get; set; } = new EmailTemplate
        {
            Title = "[{0}] 驗證您的信箱",
            Body = "<a href='{0}' target='_blank'>點我</a>驗證您的信箱。",
            IsHtml = true
        };

        public int MaxLoginCount { get; set; } = 1;

        public TimeSpan LoginTokenPeriod { get; set; } = TimeSpan.FromHours(12);

        public class EmailTemplate
        {
            public string Title { get; set; }

            public string Body { get; set; }

            public bool IsHtml { get; set; }
        }
    }
}
