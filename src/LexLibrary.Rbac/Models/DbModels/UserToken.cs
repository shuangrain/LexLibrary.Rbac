using System;
using System.ComponentModel.DataAnnotations;

namespace LexLibrary.Rbac.Models.DbModels
{
    public class UserToken : ValidatableObject
    {
        public UserToken()
        {
            Value = Guid.NewGuid().ToString().Replace("-", string.Empty);
            CreateDT = DateTime.Now;
        }

        public long Id { get; set; }

        public long UserId { get; set; }

        [Required]
        [MaxLength(128)]
        public string Name { get; set; }

        [Required]
        [MaxLength(32)]
        public string Value { get; set; }

        public DateTime CreateDT { get; set; }

        public DateTime ExpiredDT { get; set; }

        public User User { get; set; }
    }
}
