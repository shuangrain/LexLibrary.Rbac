using LexLibrary.Rbac.Extensions;
using LexLibrary.Rbac.Models.Consts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace LexLibrary.Rbac.Attributes
{
    public class LexLibraryRbacAuthorizedRedirectAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var request = context.HttpContext.Request;

            if (request.IsLogin())
            {
                string url = null;

                if ((request.Query?.ContainsKey(ApplicationConst.ReturnUrl)).GetValueOrDefault())
                {
                    url = request.Query[ApplicationConst.ReturnUrl];
                }
                else if (request.HasFormContentType && (request.Form?.ContainsKey(ApplicationConst.ReturnUrl)).GetValueOrDefault())
                {
                    url = request.Form[ApplicationConst.ReturnUrl];
                }
                else
                {
                    url = request.GetBaseUrl();
                }

                context.Result = new RedirectResult(url);
                return;
            }
        }
    }
}
