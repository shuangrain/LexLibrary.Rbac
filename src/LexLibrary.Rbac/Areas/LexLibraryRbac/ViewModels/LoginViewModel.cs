using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LexLibrary.Rbac.Areas.LexLibraryRbac.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [MinLength(4)]
        [MaxLength(128)]
        public string Account { get; set; }

        [Required]
        [MinLength(4)]
        [MaxLength(128)]
        public string Password { get; set; }

        public bool IsPersistent { get; set; } = true;

        public string ReturnUrl { get; set; }
    }
}
