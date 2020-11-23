using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MockSchoolManagement.Models;
using MockSchoolManagement.ViewModels;

namespace MockSchoolManagement.Controllers
{
    /// <summary>
    /// 账户控制器
    /// </summary>
    [AllowAnonymous]//允许匿名访问，不需要验证
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<AccountController> logger)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._logger = logger;
        }

        //打开注册页面
        [HttpGet]
        public ViewResult Register()
        {
            return View();
        }

        //注册方法 
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    City = model.City
                };
                //保存数据到AspNetUsers
                var result = await _userManager.CreateAsync(user, model.Password);
                //成功
                if (result.Succeeded)
                {
                    //生成电子邮箱确认令牌
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //生成电子邮箱的确认连接
                    var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);
                    //需要注入 ILogger<AccountController> _logger 服务，记录生成的url链接
                    _logger.Log(LogLevel.Warning, confirmationLink);

                    //如果用户是Admin角色，跳转用户列表视图
                    if (_signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
                    {
                        return RedirectToAction("ListUsers", "Admin");
                    }
                    ViewBag.ErrorTitle = "注册成功";
                    ViewBag.ErrorMessage = $"在您登入系统前，我们已经给您发了一份邮件，需要您先进行邮件验证，单机确认链接即可完成。";
                    return View("Error");
                }
                //如果有错误，添加到ModelState对象中
                //将由验证摘要标记助手显示到视图中
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        //注销
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        //打开登录视图
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            LoginViewModel model = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            return View(model);
        }

        //登录
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            //获取外部身份验证集合
            model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.UserName);
                if (user != null && !user.EmailConfirmed && (await _userManager.CheckPasswordAsync(user, model.Password)))
                {
                    ModelState.AddModelError(string.Empty, "您的电子邮箱还未进行验证");
                    return View(model);
                }
                //在PasswordSignInAsync()中，我们讲最后一个参数从false改为true，用于启动账户锁定
                //每次登录失败后，都会将 AspNetUsers 表中的AccessFailedCount增加1，等于5时将会锁定
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe,true);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        if (Url.IsLocalUrl(returnUrl))//判断是否为本地url
                        {
                            return Redirect(returnUrl);
                        }
                    }
                    return RedirectToAction("Index", "Home");
                }
                //如果账户状态为IsLockedOut，那么重定向到AccountLocked视图，给予提示
                if (result.IsLockedOut)
                {
                    return View("AccountLocked");
                }
                ModelState.AddModelError(string.Empty, "登录失败，请重试。");
            }
            return View(model);
        }

        //第三方登录
        [HttpPost]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        //第三方登录回调
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            LoginViewModel loginViewModel = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            if (remoteError != null)
            {
                ModelState.AddModelError("", $"第三方登录提供程序错误：{remoteError}");
                return View("Login", loginViewModel);
            }
            //从第三方登录提供商，获取关于用户的登录信息
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError("", "加载第三方登录信息出错。");
                return View("Login", loginViewModel);
            }
            // 获取邮箱地址
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            ApplicationUser user = null;
            if (email != null)
            {
                //通过邮箱查找用户是否存在
                user = await _userManager.FindByEmailAsync(email);
                if (user != null && !user.EmailConfirmed)
                {
                    ModelState.AddModelError("", "您的电子邮件还未进行验证。");
                    return View("Login", loginViewModel);
                }
            }
            //如果用户之前已经登录过，则会在 AspNetUserLogins 有记录，这个时候无需创建新的记录，直接使用当前记录登录
            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            //如果 AspNetUserLogins 没有记录，需要创建新记录
            else
            {
                if (email != null)
                {
                    if (user == null)
                    {
                        user = new ApplicationUser
                        {
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                        };
                        //如果不存在，则创建一个用户，但是这用户没有密码
                        await _userManager.CreateAsync(user);

                        //生成电子邮箱令牌
                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        //生成确认链接
                        var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);
                        //记录生成的确认链接
                        _logger.Log(LogLevel.Warning, confirmationLink);
                        ViewBag.ErrorTitle = "注册成功";
                        ViewBag.ErrorMessage = $"在您登入系统前，我们已经给您发了一份邮件，需要您先进行邮件验证，单机确认链接即可完成。";
                        return View("Error");
                    }
                    //在 AspNetUserLogins 中添加一行用户数据，然后将当前用户登录到系统中
                    await _userManager.AddLoginAsync(user, info);
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                //如果我们获取不到邮箱，需要将请求重定向到错误视图
                ViewBag.ErrorTitle = $"我们无法从提供商：{info.LoginProvider} 中解析到读者的邮箱地址。";
                ViewBag.ErrorMessage = "请通过联系648946942@qq.com寻求技术支持。";
                return View("Error");
            }
        }

        /// <summary>
        /// 远程验证，会调用jquery提供的remote()方法，发送一个ajax，返回需要JSON响应。
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"邮箱：{email} 已经被注册使用了。");
            }
        }

        //确认邮箱
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"当前{userId}无效";
                return View("NotFound");
            }
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return View();
            }
            ViewBag.ErrorTitle = "您的电子邮箱还未进行验证";
            return View("Error");
        }

        //激活用户邮箱页面
        [HttpGet]
        public IActionResult ActivateUserEmail()
        {
            return View();
        }

        //激活用户邮箱
        [HttpPost]
        public async Task<IActionResult> ActivateUserEmail(EmailAddressViewModel model)
        {
            if (ModelState.IsValid)
            {
                //通过邮箱查询用户
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    if (!await _userManager.IsEmailConfirmedAsync(user))
                    {
                        //生成电子邮箱令牌
                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        //生成确认链接
                        var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);
                        //记录生成的确认链接
                        _logger.Log(LogLevel.Warning, confirmationLink);
                        ViewBag.Message = $"如果您在我们系统有注册账户，我们已经给您发了一份邮件，需要您先进行邮件验证，请前往邮箱激活您的账户。";
                        //重定向到忘记邮箱确认视图
                        return View("ActivateUserEmailConfirmation", ViewBag.Message);
                    }
                }
            }
            ViewBag.Message = "请确认邮箱是否存在异常，现在我们无法给您发送激活链接。";
            //为了避免账户枚举和暴力攻击，不进行用户不存在活邮箱未验证的提示
            return View("ActivateUserEmailConfirmation", ViewBag.Message);
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(EmailAddressViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                //有用户并且邮箱已经验证
                if (user != null && await _userManager.IsEmailConfirmedAsync(user))
                {
                    //生成重置密码的token
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    //生成重置密码的链接
                    var passwordResetLink = Url.Action("ResetPassword", "Account", new { email = model.Email, token }, Request.Scheme);
                    //将重置密码记录在日志中
                    _logger.Log(LogLevel.Warning, passwordResetLink);
                    //重定向到重置密码视图
                    return View("ForgotPasswordConfirmation");
                }
                //为了避免账户枚举和暴力攻击，不进行用户不存在活邮箱未验证的提示
                return View("ForgotPasswordConfirmation", ViewBag.Message);
            }
            return View(model);
        }

        //重置密码页面
        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
            {
                ModelState.AddModelError("", "无效的重置令牌");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                //有用户并且邮箱已经验证
                if (user != null)
                {
                    //重置用户密码
                    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
                    if (result.Succeeded)
                    {
                        //重置成功后，如果当前用户被锁定，则设置该账户锁定结束时间为当前UTC日期时间，这样就能用新密码登录
                        if (await _userManager.IsLockedOutAsync(user))
                        {
                            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
                            //DateTimeOffset值的是UTC日期时间，即格林威治时间
                        }
                        return View("ResetPasswordConfirmation");
                    }
                    //显示验证错误信息，当令牌已使用或密码复杂性，等不符合标准时
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }
                // 为了避免账户枚举和暴力攻击，不要提示用户不存在
                return View("ResetPasswordConfirmation");
            }
            //如果验证未通过，显示验证错误
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);
            //判断当前用户是否有密码
            var userHasPassword = await _userManager.HasPasswordAsync(user);
            if (!userHasPassword)
            {
                return RedirectToAction("AddPassword");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login");
                }
                //使用 ChangePasswordAsync() 修改密码
                var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View();
                }
                //修改成功，刷新登录cookie
                await _signInManager.RefreshSignInAsync(user);
                return View("ChangePasswordConfirmation");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> AddPassword()
        {
            var user = await _userManager.GetUserAsync(User);

            var userHasPassword = await _userManager.HasPasswordAsync(user);
            if (userHasPassword)
            {
                return RedirectToAction("ChangePassword");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddPassword(AddPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                //添加新密码
                var result = await _userManager.AddPasswordAsync(user, model.NewPassword);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View();
                }
                //添加成功，刷新登录cookie
                await _signInManager.RefreshSignInAsync(user);
                return View("AddPasswordConfirmation");
            }
            return View(model);
        }
    }
}