using LexLibrary.Rbac.Abstractions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexLibrary.Rbac.Sample.Services
{
    public class CryptHelper : ICryptHelper
    {
        private readonly IConfiguration _configuration = null;

        public CryptHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Decrypt(string text)
        {
            return text;
        }

        public string Encrypt(string text)
        {
            return text;
        }

        public string HashPassword(string text)
        {
            return text;
        }

        public bool ValidatePassword(string password, string correctHash)
        {
            return password == correctHash;
        }
    }
}
