using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LexLibrary.Rbac.Extensions
{
    public static class StringExtensions
    {
        public static bool TryParseJson(this string json, out JToken token)
        {
            try
            {
                token = JToken.Parse(json);
                return true;
            }
            catch
            {
                token = null;
                return false;
            }
        }
    }
}
