using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LexLibrary.Rbac.Abstractions
{
    public interface IEmailSender
    {
        bool SendMail(string to, string title, string body, bool isHtml);
    }
}
