using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LexLibrary.Rbac.Models.DbModels
{
    public class User : ValidatableObject
    {
        public User()
        {
            CreateDT = DateTime.Now;
            ModifyDT = DateTime.Now;
            IsEnable = true;
        }

        public long Id { get; set; }

        [Required]
        [MaxLength(128)]
        public string Account { get; set; }

        [Required]
        [MaxLength(128)]
        public string Password { get; set; }

        [Required]
        [MaxLength(128)]
        public string DisplayName { get; set; }

        [MaxLength(128)]
        public string Email { get; set; }

        public bool IsEmailConfirmed { get; set; }

        public DateTime CreateDT { get; set; }

        public DateTime ModifyDT { get; set; }

        public bool IsEnable { get; set; }

        public IList<UserRoleMapping> UserRoleMappings { get; set; } = new List<UserRoleMapping>();

        public IList<UserToken> UserTokens { get; set; } = new List<UserToken>();
    }
}
