using LuduStack.Application;
using LuduStack.Application.Formatters;
using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.User;
using LuduStack.Application.ViewModels.UserPreferences;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Identity.Models;
using LuduStack.Infra.CrossCutting.Identity.Models.AccountViewModels;
using LuduStack.Infra.CrossCutting.Identity.Services;
using LuduStack.Web.Controllers.Base;
using LuduStack.Web.Exceptions;
using LuduStack.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static LuduStack.Infra.CrossCutting.Identity.Services.EmailSenderExtensions;

namespace LuduStack.Web.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class AccountController : SecureBaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IProfileAppService profileAppService;
        private readonly ILogger _logger;

        private readonly IWebHostEnvironment hostingEnvironment;

        public IConfiguration Configuration { get; }

        private readonly string envName;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IProfileAppService profileAppService,
            ILogger<AccountController> logger,
            IWebHostEnvironment hostingEnvironment,
            IConfiguration configuration) : base()
        {
            _userManager = userManager;
            _signInManager = signInManager;
            this.profileAppService = profileAppService;
            _logger = logger;
            this.hostingEnvironment = hostingEnvironment;
            Configuration = configuration;

            envName = string.Format("env-{0}", hostingEnvironment.EnvironmentName);
        }

        [TempData]
        public string ErrorMessage { get; set; }

        [HttpGet]
        [AllowAnonymous]
        [Route("/account/login")]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded && !string.IsNullOrWhiteSpace(model.UserName))
                {
                    return await ManageSuccessfullLogin(model, returnUrl);
                }

                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction(nameof(LoginWith2fa), new { returnUrl, model.RememberMe });
                }

                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToAction(nameof(Lockout));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private async Task<IActionResult> ManageSuccessfullLogin(LoginViewModel model, string returnUrl)
        {
            ApplicationUser user = await _userManager.FindByNameAsync(model.UserName);

            if (user != null && !string.IsNullOrWhiteSpace(user.UserName))
            {
                SetEmailConfirmed(user);

                await SetProfileOnSession(new Guid(user.Id), user.UserName);

                await SetStaffRoles(user);

                await SetPreferences(user);

                await SetCache(user);
            }

            string logMessage = string.Format("User {0} logged in.", model.UserName);

            if (EnvName.Equals(Constants.ProductionEnvironmentName))
            {
                await NotificationSender.SendTeamNotificationAsync(logMessage);
            }

            _logger.LogInformation(logMessage);

            return RedirectToLocal(returnUrl);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWith2fa(bool rememberMe, string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            ApplicationUser user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                throw new CustomApplicationException($"Unable to load two-factor authentication user.");
            }

            LoginWithTwoFactorViewModel model = new LoginWithTwoFactorViewModel { RememberMe = rememberMe };
            ViewData["ReturnUrl"] = returnUrl;

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWith2fa(LoginWithTwoFactorViewModel model, bool rememberMe, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            ApplicationUser user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new CustomApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            string authenticatorCode = model.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, model.RememberMachine);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID {UserId} logged in with 2fa.", user.Id);
                return RedirectToLocal(returnUrl);
            }
            else if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                _logger.LogWarning("Invalid authenticator code entered for user with ID {UserId}.", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
                return View();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithRecoveryCode(string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            ApplicationUser user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new CustomApplicationException($"Unable to load two-factor authentication user.");
            }

            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWithRecoveryCode(LoginWithRecoveryCodeViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            ApplicationUser user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new CustomApplicationException($"Unable to load two-factor authentication user.");
            }

            string recoveryCode = model.RecoveryCode.Replace(" ", string.Empty);

            Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID {UserId} logged in with a recovery code.", user.Id);
                return RedirectToLocal(returnUrl);
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                _logger.LogWarning("Invalid recovery code entered for user with ID {UserId}", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");
                return View();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("/account/register")]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            MvcRegisterViewModel model = new MvcRegisterViewModel();

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(MvcRegisterViewModel model, string returnUrl = null)
        {
            bool reCaptchaValid = IsReCaptchValid();

            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid && reCaptchaValid)
            {
                ApplicationUser user = new ApplicationUser { UserName = model.UserName, Email = model.Email };

                IdentityResult result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    ProfileViewModel profile = profileAppService.GenerateNewOne(ProfileType.Personal);
                    profile.UserId = new Guid(user.Id);
                    profile.Handler = model.UserName;
                    await profileAppService.Save(CurrentUserId, profile);

                    await UploadFirstAvatar(profile.UserId, ProfileType.Personal);

                    await SetStaffRoles(user);

                    await SetPreferences(user);

                    string logMessage = string.Format("User {0} created a new account with password.", model.UserName);

                    if (EnvName.Equals(Constants.ProductionEnvironmentName))
                    {
                        await NotificationSender.SendTeamNotificationAsync(logMessage);
                    }

                    _logger.LogInformation(logMessage);

                    string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    string callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    await NotificationSender.SendEmailConfirmationAsync(model.Email, callbackUrl);

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation("User logged in with password.");
                    return RedirectToLocal(returnUrl);
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public bool IsReCaptchValid()
        {
            if (envName.ToLower().Equals("env-development"))
            {
                return true;
            }

            bool result = false;
            Microsoft.Extensions.Primitives.StringValues captchaResponse = Request.Form["g-recaptcha-response"];
            string secretKey = Configuration["ReCaptcha:SecretKey"];
            string apiUrl = "https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}";
            string requestUri = string.Format(apiUrl, secretKey, captchaResponse);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUri);

            using (WebResponse response = request.GetResponse())
            {
                using (StreamReader stream = new StreamReader(response.GetResponseStream()))
                {
                    JObject jResponse = JObject.Parse(stream.ReadToEnd());
                    bool isSuccess = jResponse.Value<bool>("success");
                    result = isSuccess;
                }
            }
            return result;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            string redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            AuthenticationProperties properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToAction(nameof(Login));
            }
            ExternalLoginInfo info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            // Sign in the user with this external login provider if the user already has a login.
            Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                string email = info.Principal.FindFirstValue(ClaimTypes.Email);
                ApplicationUser existingUser = await _userManager.FindByEmailAsync(email);

                if (existingUser != null)
                {
                    string logMessage = string.Format("User {0} logged in with {1} provider.", existingUser.UserName, info.LoginProvider);

                    if (EnvName.Equals(Constants.ProductionEnvironmentName))
                    {
                        await NotificationSender.SendTeamNotificationAsync(logMessage);
                    }

                    _logger.LogInformation(logMessage);

                    await SetProfileOnSession(new Guid(existingUser.Id), existingUser.UserName);

                    await SetStaffRoles(existingUser);

                    await SetPreferences(existingUser);
                }

                return RedirectToLocal(returnUrl);
            }

            if (result.IsLockedOut)
            {
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.LoginProvider;
                ViewData["ButtonText"] = SharedLocalizer["Register"];
                string text = "You've successfully authenticated with your external account. Please choose a username to use and click the Register button to finish logging in.";

                string email = info.Principal.FindFirstValue(ClaimTypes.Email);

                MvcExternalLoginViewModel model = new MvcExternalLoginViewModel { Email = email };

                ApplicationUser existingUser = await _userManager.FindByEmailAsync(email);
                if (existingUser != null)
                {
                    model.UserExists = true;
                    model.Username = existingUser.UserName;

                    text = "Oh! It looks like you already have a user registered here with us. Check your info below and confirm to link your account to your external account.";

                    ViewData["ButtonText"] = SharedLocalizer["Link Acounts"];
                }
                else
                {
                    model.ProfileName = SelectName(info);
                }

                ViewData["RegisterText"] = SharedLocalizer[text];

                return View("ExternalLogin", model);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(MvcExternalLoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result;

                // Get the information about the user from the external login provider
                ExternalLoginInfo externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();
                if (externalLoginInfo == null)
                {
                    throw new CustomApplicationException("Error loading external login information during confirmation.");
                }

                ApplicationUser user = new ApplicationUser { UserName = model.Username, Email = model.Email };
                ApplicationUser existingUser = await _userManager.FindByEmailAsync(model.Email);

                if (existingUser != null)
                {
                    user = existingUser;
                }
                else
                {
                    await _userManager.CreateAsync(user);
                }

                result = await _userManager.AddLoginAsync(user, externalLoginInfo);

                if (result.Succeeded)
                {
                    return await HandleSucessfullExternalLogin(returnUrl, externalLoginInfo, user, existingUser);
                }

                AddErrors(result);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(nameof(ExternalLogin), model);
        }

        private async Task<IActionResult> HandleSucessfullExternalLogin(string returnUrl, ExternalLoginInfo externalLoginInfo, ApplicationUser user, ApplicationUser existingUser)
        {
            if (existingUser == null)
            {
                await SetInitialRoles(user);
            }
            else
            {
                SetEmailConfirmed(existingUser);
                await SetStaffRoles(user);
            }

            await SetPreferences(user);

            Guid userGuid = new Guid(user.Id);
            ProfileViewModel profile = await profileAppService.GetByUserId(userGuid, ProfileType.Personal);
            if (profile == null)
            {
                profile = profileAppService.GenerateNewOne(ProfileType.Personal);
                profile.UserId = userGuid;

                profile.Handler = user.UserName;

                profile.Name = SelectName(externalLoginInfo);
            }

            await SetExternalProfilePicture(externalLoginInfo, user, profile);

            if (string.IsNullOrWhiteSpace(profile.ProfileImageUrl) || profile.ProfileImageUrl == Constants.DefaultAvatar)
            {
                await UploadFirstAvatar(profile.UserId, ProfileType.Personal);
            }

            await profileAppService.Save(CurrentUserId, profile);

            await SetProfileOnSession(new Guid(user.Id), user.UserName);

            await _signInManager.SignInAsync(user, isPersistent: false);

            string logMessage = string.Format("User {0} linked a {1} account.", user.UserName, externalLoginInfo.LoginProvider);
            if (existingUser == null)
            {
                logMessage = string.Format("User {0} registered with a {1} account.", user.UserName, externalLoginInfo.LoginProvider);
            }

            if (EnvName.Equals(Constants.ProductionEnvironmentName))
            {
                await NotificationSender.SendTeamNotificationAsync(logMessage);
            }

            _logger.LogInformation(logMessage);

            return RedirectToLocal(returnUrl);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            ApplicationUser user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new CustomApplicationException($"Unable to load user with ID '{userId}'.");
            }

            IdentityResult result = await _userManager.ConfirmEmailAsync(user, code);

            SetEmailConfirmed(user);

            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await _userManager.FindByEmailAsync(model.Email);
                bool emailAlreadyConfirmed = await _userManager.IsEmailConfirmedAsync(user);
                if (user == null || !emailAlreadyConfirmed)
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToAction(nameof(ForgotPasswordConfirmation));
                }

                // For more information on how to enable account confirmation and password reset please
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                string code = await _userManager.GeneratePasswordResetTokenAsync(user);
                string callbackUrl = Url.ResetPasswordCallbackLink(user.Id, code, Request.Scheme);

                await NotificationSender.SendEmailPasswordResetAsync(model.Email, callbackUrl);
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
            {
                throw new CustomApplicationException("A code must be supplied for password reset.");
            }
            ResetPasswordViewModel model = new ResetPasswordViewModel { Code = code };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            ApplicationUser user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            IdentityResult result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            AddErrors(result);
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ValidateUserName(string UserName, string Email)
        {
            OperationResultVo result;

            try
            {
                Regex pattern = new Regex("^(?=.{3,32}$)(?![-.])(?!.*[-.]{2})[a-zA-Z0-9-.]+(?<![-.])$");

                Match match = pattern.Match(UserName);

                if (!match.Success)
                {
                    return Json(SharedLocalizer["Username not available!"].ToString());
                }

                ApplicationUser user = await UserManager.FindByNameAsync(UserName);

                if (user == null)
                {
                    return Json(true);
                }
                else
                {
                    if (user.Email.Equals(Email))
                    {
                        return Json(true);
                    }

                    return Json(SharedLocalizer["Oops! Someone already took that username!"].ToString());
                }
            }
            catch (Exception ex)
            {
                result = new OperationResultVo(false);
                _logger.Log(LogLevel.Error, ex, ex.Message);
            }

            return Json(result);
        }

        private async Task SetPreferences(ApplicationUser user)
        {
            UserPreferencesViewModel preferences = await UserPreferencesAppService.GetByUserId(new Guid(user.Id));

            SetUserPreferences(preferences);
        }

        #region Helpers

        // HACK replace by default admin user
        private async Task SetStaffRoles(ApplicationUser user)
        {
            int userCount = _userManager.Users.Count();

            IList<string> userRoles = await _userManager.GetRolesAsync(user);

            bool userIsMember = userRoles.Contains(Roles.Member.ToString());

            if (!userIsMember)
            {
                await _userManager.AddToRoleAsync(user, Roles.Member.ToString());
            }

            if (userCount == 1)
            {
                bool userIsAdmin = userRoles.Contains(Roles.Administrator.ToString());

                if (!userIsAdmin)
                {
                    await _userManager.AddToRoleAsync(user, Roles.Administrator.ToString());
                }
            }
        }

        private async Task SetInitialRoles(ApplicationUser user)
        {
            await _userManager.AddToRoleAsync(user, Roles.Member.ToString());
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        private async Task SetExternalProfilePicture(ExternalLoginInfo info, ApplicationUser user, ProfileViewModel profile)
        {
            UploadResultVo result = new UploadResultVo(true, MediaType.Image, Constants.DefaultAvatar, "No external profile picture found. Setting default avatar.");

            if (string.IsNullOrWhiteSpace(profile.ProfileImageUrl) || profile.ProfileImageUrl.Equals(Constants.DefaultAvatar))
            {
                if (info.LoginProvider == "Facebook")
                {
                    string nameIdentifier = info.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
                    string pictureUrl = $"https://graph.facebook.com/{nameIdentifier}/picture?type=large";

                    result = await UploadProfilePicture(user.Id, pictureUrl);
                }
                else if (info.LoginProvider == "Google" && info.Principal.HasClaim(x => x.Type == "urn:google:picture"))
                {
                    string pictureUrl = info.Principal.FindFirstValue("urn:google:picture");

                    result = await UploadProfilePicture(user.Id, pictureUrl);
                }
                else if (info.LoginProvider == "Github")
                {
                    string nameIdentifier = info.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
                    string pictureUrl = $"https://avatars.githubusercontent.com/u/{nameIdentifier}";

                    result = await UploadProfilePicture(user.Id, pictureUrl);
                }
            }

            profile.ProfileImageUrl = result.Filename.Equals(Constants.DefaultAvatar) ? result.Filename : Constants.DefaultImagePath + result.Filename;
        }

        private async Task<UploadResultVo> UploadProfilePicture(string userId, string pictureUrl)
        {
            byte[] thumbnailBytes;

            using (HttpClient httpClient = new HttpClient())
            {
                string filename = userId + "_" + ProfileType.Personal.ToString();
                thumbnailBytes = await httpClient.GetByteArrayAsync(pictureUrl);

                string extension = GetFileExtension(pictureUrl);

                UploadResultVo result = await base.UploadImage(new Guid(userId), ImageType.ProfileImage, filename, extension, thumbnailBytes, envName);

                if (!result.Success || string.IsNullOrWhiteSpace(result.Filename))
                {
                    result.Filename = Constants.DefaultAvatar;
                }

                return result;
            }
        }

        private async Task SetCache(ApplicationUser user)
        {
            Guid key = new Guid(user.Id);
            UserProfileEssentialVo cachedProfile = await profileAppService.GetEssentialUserProfileWithCache(key);

            if (cachedProfile == null)
            {
                UserProfileEssentialVo profile = await profileAppService.GetEssentialUserProfileWithCache(key);
                if (profile != null)
                {
                    profileAppService.SetProfileCache(key, profile);
                }
            }
        }

        private async Task UploadFirstAvatar(Guid userId, ProfileType type)
        {
            string fileName = string.Format("{0}_{1}", userId, type);

            string defaultImageNotRooted = UrlFormatter.GetDefaultImage(ImageType.ProfileImage);

            string retorno = Path.Combine(hostingEnvironment.WebRootPath, defaultImageNotRooted);

            byte[] bytes = System.IO.File.ReadAllBytes(retorno);

            fileName = fileName.Split('.').First();

            string extension = GetFileExtension(fileName);

            await base.UploadImage(userId, ImageType.ProfileImage, fileName, extension, bytes, envName);
        }

        private static string SelectName(ExternalLoginInfo info)
        {
            string name = info.Principal.FindFirstValue(ClaimTypes.Name);

            switch (info.LoginProvider)
            {
                case "Github":
                    name = info.Principal.FindFirstValue("urn:github:name");
                    break;
            }

            return name;
        }

        #endregion Helpers
    }
}