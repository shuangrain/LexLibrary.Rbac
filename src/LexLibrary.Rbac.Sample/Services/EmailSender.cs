using LexLibrary.Rbac.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexLibrary.Rbac.Sample.Services
{
    public class EmailSender : IEmailSender
    {
        public bool SendMail(string to, string title, string body, bool isHtml)
        {
            return true;
        }
    }
}
