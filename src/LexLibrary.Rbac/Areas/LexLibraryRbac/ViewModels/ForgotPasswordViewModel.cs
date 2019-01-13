using System.ComponentModel.DataAnnotations;

namespace LexLibrary.Rbac.Areas.LexLibraryRbac.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [MinLength(4)]
        [MaxLength(128)]
        public string Account { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(128)]
        public string Email { get; set; }
    }
}
