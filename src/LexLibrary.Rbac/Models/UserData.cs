using System;
using System.Collections.Generic;
using System.Text;

namespace LexLibrary.Rbac.Models
{
    public class UserData
    {
        public long Id { get; set; }

        public string DisplayName { get; set; }

        public string LoginToken { get; set; }

        public bool IsPersistent { get; set; }
    }
}
