using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LexLibrary.Rbac.Sample.Models;
using LexLibrary.Rbac.Attributes;
using Microsoft.AspNetCore.Authorization;

namespace LexLibrary.Rbac.Sample.Controllers
{
    [LexLibraryRbacAuthorize(RoleIds = "1")]
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult TestPermission()
        {
            return View();
        }
    }
}
