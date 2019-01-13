using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LexLibrary.Rbac.Areas.LexLibraryRbac.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string Secret { get; set; }

        [Required]
        [MinLength(4)]
        [MaxLength(128)]
        public string Password { get; set; }

        [Required]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
