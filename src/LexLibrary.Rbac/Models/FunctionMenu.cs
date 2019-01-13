using System;
using System.Collections.Generic;
using System.Text;

namespace LexLibrary.Rbac.Models
{
    public class FunctionMenu
    {
        public int Id { get; set; }

        public string IconClass { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public bool IsNewTab { get; set; }

        public IEnumerable<FunctionMenu> ChildFunctionMenu { get; set; }
    }
}
