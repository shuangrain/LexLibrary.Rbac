using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LexLibrary.Rbac.Models.DbModels
{
    public class Role : ValidatableObject
    {
        public Role()
        {
            CreateDT = DateTime.Now;
            ModifyDT = DateTime.Now;
            IsEnable = true;
        }

        public int Id { get; set; }

        [Required]
        [MaxLength(128)]
        public string Name { get; set; }

        public DateTime CreateDT { get; set; }

        public DateTime ModifyDT { get; set; }

        public bool IsEnable { get; set; }

        public IList<UserRoleMapping> UserRoleMappings { get; set; } = new List<UserRoleMapping>();

        public IList<RoleFunctionMapping> RoleFunctionMappings { get; set; } = new List<RoleFunctionMapping>();
    }
}
