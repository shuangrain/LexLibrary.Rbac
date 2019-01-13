using System;
using System.Collections.Generic;
using System.Text;

namespace LexLibrary.Rbac.Abstractions
{
    public interface ICryptHelper
    {
        string Encrypt(string text);

        string Decrypt(string text);

        string HashPassword(string text);

        bool ValidatePassword(string password, string correctHash);
    }
}
