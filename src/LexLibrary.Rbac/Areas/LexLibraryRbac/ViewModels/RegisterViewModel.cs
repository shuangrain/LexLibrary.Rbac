using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LexLibrary.Rbac.Areas.LexLibraryRbac.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [MinLength(4)]
        [MaxLength(128)]
        public string Account { get; set; }

        [Required]
        [MinLength(4)]
        [MaxLength(128)]
        public string Password { get; set; }

        [Required]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }

        [Required]
        [MaxLength(128)]
        public string DisplayName { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(128)]
        public string Email { get; set; }
    }
}
