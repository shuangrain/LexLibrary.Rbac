using LexLibrary.Rbac.Models;
using System.Collections.Generic;

namespace LexLibrary.Rbac.Services
{
    public interface IUserManager
    {
        bool IsLogin { get; }
        UserData UserData { get; }

        BaseResult ChangePassword(long userId, string password);

        BaseResult ConfirmEmail(string d);

        BaseResult ForgotPassword(string account, string email);

        IEnumerable<FunctionMenu> GetFunctionMenu();

        bool HasPermission(IEnumerable<int> functionIds = null, IEnumerable<int> roleIds = null);

        BaseResult Login(string account, string password, bool isPersistent);

        void Logout();

        BaseResult Register(string account, string password, string email, string displayName);

        BaseResult ValidateToken(string type, string d, out long userId);
    }
}