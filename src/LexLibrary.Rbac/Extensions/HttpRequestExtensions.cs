using LexLibrary.Rbac.Models;
using LexLibrary.Rbac.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace LexLibrary.Rbac.Extensions
{
    public static class HttpRequestExtensions
    {
        /// <summary>
        /// 使用者資料
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public static UserData GetUserData(this HttpRequest httpRequest)
        {
            var userManage = httpRequest.HttpContext.RequestServices.GetService<IUserManager>();
            return userManage.UserData;
        }

        /// <summary>
        /// 是否已登入
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public static bool IsLogin(this HttpRequest httpRequest)
        {
            var userManage = httpRequest.HttpContext.RequestServices.GetService<IUserManager>();
            return userManage.IsLogin;
        }

        public static string GetBaseUrl(this HttpRequest request)
        {
            return string.Format("{0}://{1}", request.Scheme, request.Host.ToString());
        }
    }
}
