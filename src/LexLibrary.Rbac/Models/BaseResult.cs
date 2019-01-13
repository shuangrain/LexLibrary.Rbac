using System;
using System.Collections.Generic;
using System.Text;

namespace LexLibrary.Rbac.Models
{
    public class BaseResult
    {
        public bool IsSuccess { get; set; }

        public string RtnMsg { get; set; }

        public static BaseResult Ok()
        {
            return new BaseResult
            {
                IsSuccess = true,
                RtnMsg = "成功"
            };
        }
    }
}
