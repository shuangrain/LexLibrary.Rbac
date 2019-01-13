using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LexLibrary.Rbac.Models.DbModels
{
    public class Function : ValidatableObject
    {
        public Function()
        {
            CreateDT = DateTime.Now;
            ModifyDT = DateTime.Now;
            IsEnable = true;
        }

        public int Id { get; set; }

        public int? ParentId { get; set; }

        public int OrderSeq { get; set; }

        [Required]
        [MaxLength(128)]
        public string Name { get; set; }

        [MaxLength(128)]
        public string Url { get; set; }

        [MaxLength(128)]
        public string IconClass { get; set; }

        public bool IsNewTab { get; set; }

        public DateTime CreateDT { get; set; }

        public DateTime ModifyDT { get; set; }

        public bool IsEnable { get; set; }

        public Function ChildFunction { get; set; }

        public IList<Function> ChildFunctions { get; set; } = new List<Function>();

        public IList<RoleFunctionMapping> RoleFunctionMappings { get; set; } = new List<RoleFunctionMapping>();
    }
}
