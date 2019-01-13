using System;
using System.Collections.Generic;
using System.Text;

namespace LexLibrary.Rbac.Models
{
    public class ValidateToken
    {
        public long UserId { get; set; }

        public string Token { get; set; }

        public string Email { get; set; }
    }
}
