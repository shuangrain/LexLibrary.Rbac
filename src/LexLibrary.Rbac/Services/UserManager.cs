using LexLibrary.Rbac.Abstractions;
using LexLibrary.Rbac.Extensions;
using LexLibrary.Rbac.Models;
using LexLibrary.Rbac.Models.Consts;
using LexLibrary.Rbac.Models.DbModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using static LexLibrary.Rbac.Models.LexLibraryRbacSetting;

namespace LexLibrary.Rbac.Services
{
    public class UserManager : IUserManager
    {
        #region Public Property

        public bool IsLogin
        {
            get
            {
                if (_userData == null && !tryGetLoginCookie(out _userData))
                {
                    return false;
                }

                DateTime expiredDT = default(DateTime);
                bool isLogin = (_userData != null &&
                                validUserToken(_userData.Id,
                                               UserTokens.Login,
                                               _userData.LoginToken,
                                               out expiredDT));


                if (isLogin)
                {
                    double remainingSec = expiredDT.Subtract(DateTime.Now).TotalSeconds;
                    double halfSec = _setting.LoginTokenPeriod.TotalSeconds / 2;
                    if (remainingSec > 0 && remainingSec < halfSec)
                    {
                        spreadUserToken(_userData.Id, UserTokens.Login, _userData.LoginToken, _setting.LoginTokenPeriod);
                        writeLoginCookie(_userData);
                    }
                }
                else
                {
                    removeLoginCookie();
                }

                return isLogin;
            }
        }

        private UserData _userData = null;
        public UserData UserData
        {
            get
            {
                if (!IsLogin)
                {
                    return null;
                }

                return _userData;
            }
            private set
            {
                _userData = value;
            }
        }

        #endregion

        #region Ctor

        private readonly LexLibraryRbacSetting _setting = null;
        private readonly HttpContext _httpContext = null;
        private readonly IEmailSender _emailSender = null;
        private readonly ICryptHelper _cryptHelper = null;
        private readonly IRepository<User> _userRepository = null;
        private readonly IRepository<Function> _functionRepository = null;
        private readonly IRepository<UserRoleMapping> _userRoleMappingRepository = null;
        private readonly IRepository<UserToken> _userTokenRepository = null;

        public UserManager(
            LexLibraryRbacSetting setting,
            IHttpContextAccessor httpContextAccessor,
            IEmailSender emailSender,
            ICryptHelper cryptHelper,
            IRepository<User> userRepository,
            IRepository<Function> functionRepository,
            IRepository<UserRoleMapping> userRoleMappingRepository,
            IRepository<UserToken> userTokenRepository)
        {
            _setting = setting;
            _httpContext = httpContextAccessor.HttpContext;
            _emailSender = emailSender;
            _cryptHelper = cryptHelper;
            _userRepository = userRepository;
            _functionRepository = functionRepository;
            _userRoleMappingRepository = userRoleMappingRepository;
            _userTokenRepository = userTokenRepository;
        }

        #endregion

        #region Public Method

        /// <summary>
        /// 登入
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public BaseResult Login(string account, string password, bool isPersistent)
        {
            BaseResult result = new BaseResult
            {
                RtnMsg = "登入失敗，帳號或密碼錯誤。"
            };

            if (!validAccount(account, password, out User user))
            {
                return result;
            }

            string token = generateUserToken(user.Id, UserTokens.Login, _setting.LoginTokenPeriod, _setting.MaxLoginCount);
            writeLoginCookie(new UserData
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                LoginToken = token,
                IsPersistent = isPersistent
            });

            return BaseResult.Ok();
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        public void Logout()
        {
            if (IsLogin && UserData != null)
            {
                removeUserToken(UserData.Id, UserTokens.Login, UserData.LoginToken);
            }

            removeLoginCookie();
        }

        /// <summary>
        /// 取得功能清單
        /// </summary>
        /// <returns></returns>
        public IEnumerable<FunctionMenu> GetFunctionMenu()
        {
            var roleIds = getUserRoleIds(UserData.Id);
            var functions = getRoleFunctions(roleIds).ToList();
            return parseFunctionMenu(functions);
        }

        /// <summary>
        /// 判斷是否有權限
        /// </summary>
        /// <param name="functionId"></param>
        /// <returns></returns>
        public bool HasPermission(IEnumerable<int> needFunctionIds = null, IEnumerable<int> needRoleIds = null)
        {
            var roleIds = getUserRoleIds(UserData.Id);

            // admin 群組直接回傳
            if (roleIds.Contains(ApplicationConst.AdminRoleId))
            {
                return true;
            }

            var functionIds = getRoleFunctions(roleIds).Select(x => x.Id);
            if (needFunctionIds != null && needRoleIds != null)
            {
                return functionIds.Intersect(needFunctionIds).Any() && roleIds.Intersect(needRoleIds).Any();
            }
            else if (needFunctionIds != null)
            {
                return functionIds.Intersect(needFunctionIds).Any();
            }
            else if (needRoleIds != null)
            {
                return roleIds.Intersect(needRoleIds).Any();
            }
            else
            {
                throw new ArgumentNullException("傳入參數不能為空");
            }
        }

        /// <summary>
        /// 忘記密碼
        /// </summary>
        /// <param name="account"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public BaseResult ForgotPassword(string account, string email)
        {
            var result = new BaseResult
            {
                RtnMsg = "驗證失敗，帳號或信箱有誤"
            };

            if (!tryGetUserId(account, email, out long userId))
            {
                return result;
            }

            string url = generateValidateUrl(_setting.ResetPasswordPath, new ValidateToken
            {
                UserId = userId,
                Token = generateUserToken(userId, UserTokens.ResetPassword, TimeSpan.FromDays(7), 1),
                Email = email
            });

            return sendEmail(email, new EmailTemplate
            {
                Title = string.Format(_setting.ResetPasswordTemplate.Title, _setting.WebSiteName),
                Body = string.Format(_setting.ResetPasswordTemplate.Body, url),
                IsHtml = _setting.ResetPasswordTemplate.IsHtml
            });
        }

        /// <summary>
        /// 修改密碼
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public BaseResult ChangePassword(long userId, string password)
        {
            string hashPassword = _cryptHelper.HashPassword(password);

            using (var scope = new TransactionScope())
            {
                // 修改密碼
                changePassword(userId, hashPassword);

                // 移除驗證金鑰(撤回先前發布的重設密碼金鑰)
                removeUserToken(userId, UserTokens.ResetPassword);

                scope.Complete();
            }

            return BaseResult.Ok();
        }

        /// <summary>
        /// 註冊
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public BaseResult Register(string account, string password, string email, string displayName)
        {
            var result = new BaseResult
            {
                RtnMsg = "註冊失敗"
            };

            if (existAccount(account))
            {
                result.RtnMsg += "，帳號與他人重複";
                return result;
            }

            string hashPassword = _cryptHelper.HashPassword(password);

            long userId = addUser(account, hashPassword, email, displayName);

            string url = generateValidateUrl(_setting.ConfirmEmailPath, new ValidateToken
            {
                UserId = userId,
                Token = generateUserToken(userId, UserTokens.ConfirmEmail, TimeSpan.FromDays(7), 1),
                Email = email
            });

            return sendEmail(email, new EmailTemplate
            {
                Title = string.Format(_setting.ConfirmEmailTemplate.Title, _setting.WebSiteName),
                Body = string.Format(_setting.ConfirmEmailTemplate.Body, url),
                IsHtml = _setting.ConfirmEmailTemplate.IsHtml
            });
        }

        /// <summary>
        /// 驗證金鑰
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public BaseResult ValidateToken(string type, string d, out long userId)
        {
            var result = new BaseResult
            {
                RtnMsg = "驗證失敗，請重新再試。"
            };
            userId = 0;

            var model = decryptObject<ValidateToken>(d);
            if (model == null ||
                !validUserToken(model.UserId,
                                type,
                                model.Token,
                                out DateTime expiredDT))
            {
                return result;
            }

            userId = model.UserId;
            return BaseResult.Ok();
        }

        /// <summary>
        /// 確認信箱
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public BaseResult ConfirmEmail(string d)
        {
            var result = new BaseResult
            {
                RtnMsg = "驗證失敗，請重新再試。"
            };

            var model = decryptObject<ValidateToken>(d);
            if (model == null ||
                !validUserToken(model.UserId,
                                UserTokens.ConfirmEmail,
                                model.Token,
                                out DateTime expiredDT))
            {
                return result;
            }

            using (var scope = new TransactionScope())
            {
                // 驗證信箱
                confirmEmail(model.UserId, model.Email);

                // 移除驗證金鑰
                removeUserToken(model.UserId, UserTokens.ConfirmEmail);

                scope.Complete();
            }

            return BaseResult.Ok();
        }

        #endregion

        #region Private Method

        private bool validAccount(string account, string password, out User user)
        {
            user = _userRepository
                        .Query()
                        .Where(x => x.IsEnable)
                        .Where(x => x.Account == account)
                        .FirstOrDefault();
            if (user == null)
            {
                return false;
            }

            bool isValid = _cryptHelper.ValidatePassword(password, user.Password);
            if (!isValid)
            {
                user = null;
            }

            return isValid;
        }

        private void writeLoginCookie(UserData userData)
        {
            string secretData = encryptObject(userData);

            var cookies = _httpContext.Response.Cookies;
            cookies.Append(_setting.CookieName, secretData, new CookieOptions
            {
                Expires = userData.IsPersistent ? DateTime.Now.Add(_setting.LoginTokenPeriod) : (DateTime?)null,
                HttpOnly = true,
                Secure = _httpContext.Request.IsHttps
            });

            _userData = userData;
        }

        private void removeLoginCookie()
        {
            var cookies = _httpContext.Response.Cookies;
            cookies.Delete(_setting.CookieName);
        }

        private bool tryGetLoginCookie(out UserData userData)
        {
            var cookies = _httpContext.Request.Cookies;
            userData = null;

            if (!cookies.ContainsKey(_setting.CookieName) ||
                !cookies.TryGetValue(_setting.CookieName, out string secretData) ||
                string.IsNullOrWhiteSpace(secretData))
            {
                return false;
            }

            try
            {
                userData = decryptObject<UserData>(secretData);
                return (userData != null);
            }
            catch
            {
                return false;
            }
        }

        private IEnumerable<FunctionMenu> parseFunctionMenu(List<Function> functions, int? parentId = null)
        {
            return functions.Where(x => x.ParentId == parentId)
                            .OrderBy(x => x.OrderSeq)
                            .Select(x => new FunctionMenu
                            {
                                Id = x.Id,
                                Name = x.Name,
                                Url = x.Url,
                                IconClass = x.IconClass,
                                IsNewTab = x.IsNewTab,
                                ChildFunctionMenu = parseFunctionMenu(functions, x.Id)
                            });
        }

        private IQueryable<Function> getRoleFunctions(IQueryable<int> roleIds)
        {
            return _functionRepository
                        .Query()
                        .Include(x => x.RoleFunctionMappings)
                        .Where(x => x.IsEnable)
                        // admin 群組直接回傳
                        .Where(x => roleIds.Contains(ApplicationConst.AdminRoleId) ||
                                    x.RoleFunctionMappings.Any(y => roleIds.Contains(y.RoleId)));
        }

        private IQueryable<int> getUserRoleIds(long userId)
        {
            return _userRoleMappingRepository
                        .Query()
                        .Include(x => x.Role)
                        .Where(x => x.UserId == userId)
                        .Where(x => x.Role.IsEnable)
                        .Select(x => x.RoleId);
        }

        private bool tryGetUserId(string account, string email, out long userId)
        {
            long? id = _userRepository
                            .Query()
                            .Where(x => x.IsEmailConfirmed)
                            .Where(x => x.Account == account)
                            .Where(x => x.Email == email)
                            .FirstOrDefault()?
                            .Id;

            userId = id.GetValueOrDefault();
            return id.HasValue;
        }

        private BaseResult sendEmail(string email, EmailTemplate template)
        {
            var result = new BaseResult
            {
                RtnMsg = "電子郵件發送失敗，請重新再試。"
            };

            if (!_emailSender.SendMail(email, template.Title, template.Body, template.IsHtml))
            {
                return result;
            }

            result.IsSuccess = true;
            result.RtnMsg = $"電子郵件發送成功，已發送至 {email}";
            return result;
        }

        private string generateUserToken(long userId, string type, TimeSpan period, int limitCount)
        {
            var oldTokens = _userTokenRepository
                                .Query()
                                .Where(x => x.UserId == userId)
                                .Where(x => x.Name == type);

            // 移除其他授權金鑰
            if (limitCount > 0)
            {
                _userTokenRepository.RemoveRange(
                    oldTokens.OrderByDescending(x => x.CreateDT)
                             .Skip(limitCount - 1)
                );
            }

            var userToken = new UserToken
            {
                UserId = userId,
                Name = type,
                ExpiredDT = DateTime.Now.Add(period)
            };

            _userTokenRepository.Add(userToken);
            _userTokenRepository.SaveChanges();

            return userToken.Value;
        }

        private bool validUserToken(long userId, string type, string token, out DateTime expiredDT)
        {
            expiredDT = _userTokenRepository
                            .Query()
                            .Where(x => x.UserId == userId)
                            .Where(x => x.Name == type)
                            .Where(x => x.Value == token)
                            .OrderByDescending(x => x.Id)
                            .Select(x => x.ExpiredDT)
                            .FirstOrDefault();
            return expiredDT > DateTime.Now;
        }

        private void removeUserToken(long userId, string type, string token = null)
        {
            _userTokenRepository.RemoveRange(
                _userTokenRepository
                    .Query()
                    .Where(x => x.UserId == userId)
                    .Where(x => x.Name == type)
                    .Where(x => token == null || x.Value == token)
            );
            _userTokenRepository.SaveChanges();
        }

        private void spreadUserToken(long userId, string type, string token, TimeSpan period)
        {
            var userToken = _userTokenRepository
                                .Query()
                                .Where(x => x.UserId == userId)
                                .Where(x => x.Name == type)
                                .Where(x => x.Value == token)
                                .Where(x => x.ExpiredDT > DateTime.Now)
                                .FirstOrDefault();
            if (userToken == null)
            {
                return;
            }

            userToken.ExpiredDT = DateTime.Now.Add(period);
            _userTokenRepository.SaveChanges();
        }

        private bool existAccount(string account)
        {
            return _userRepository
                        .Query()
                        .Any(x => x.Account == account);
        }

        private long addUser(string account, string hashPassword, string email, string displayName)
        {
            User user = new User
            {
                Account = account,
                Password = hashPassword,
                Email = email,
                DisplayName = displayName
            };

            using (var scope = new TransactionScope())
            {
                _userRepository.Add(user);
                _userRepository.SaveChanges();

                _userRoleMappingRepository.Add(new UserRoleMapping
                {
                    UserId = user.Id,
                    RoleId = ApplicationConst.DefaultRoleId
                });
                _userRoleMappingRepository.SaveChanges();

                scope.Complete();
            }

            return user.Id;
        }

        private string generateValidateUrl(string path, object obj)
        {
            string d = encryptObject(obj);
            return _httpContext.Request.GetBaseUrl() + path.TrimEnd('/') + $"?d={HttpUtility.UrlEncode(d)}";
        }

        private string encryptObject(object obj)
        {
            string json = JsonConvert.SerializeObject(obj);
            return _cryptHelper.Encrypt(json);
        }

        private T decryptObject<T>(string d)
        {
            T model = default(T);

            string json = _cryptHelper.Decrypt(d);
            if (json.TryParseJson(out JToken jToken))
            {
                model = jToken.ToObject<T>();
            }

            return model;
        }

        private void confirmEmail(long userId, string email)
        {
            var user = _userRepository
                            .Query()
                            .FirstOrDefault(x => x.Id == userId);
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.Email = email;
            user.IsEmailConfirmed = true;
            _userRepository.SaveChanges();
        }

        private void changePassword(long userId, string hashPassword)
        {
            var user = _userRepository
                            .Query()
                            .FirstOrDefault(x => x.Id == userId);
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.Password = hashPassword;
            user.ModifyDT = DateTime.Now;
            _userRepository.SaveChanges();
        }

        #endregion
    }
}
