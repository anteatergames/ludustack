﻿using LuduStack.Application;
using LuduStack.Application.Formatters;
using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels;
using LuduStack.Application.ViewModels.User;
using LuduStack.Domain.Core.Attributes;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Extensions;
using LuduStack.Infra.CrossCutting.Abstractions;
using LuduStack.Infra.CrossCutting.Identity.Models;
using LuduStack.Web.Enums;
using LuduStack.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LuduStack.Web.Controllers.Base
{
    public class SecureBaseController : BaseController
    {
        private INotificationSender notificationSender;
        public INotificationSender NotificationSender => notificationSender ?? (notificationSender = HttpContext?.RequestServices.GetService<INotificationSender>());

        private UserManager<ApplicationUser> _userManager;
        public UserManager<ApplicationUser> UserManager => _userManager ?? (_userManager = HttpContext?.RequestServices.GetService<UserManager<ApplicationUser>>());

        private IImageStorageService _imageStorageService;
        public IImageStorageService ImageStorageService => _imageStorageService ?? (_imageStorageService = HttpContext?.RequestServices.GetService<IImageStorageService>());

        private ICookieMgrService _cookieMgrService;
        public ICookieMgrService CookieMgrService => _cookieMgrService ?? (_cookieMgrService = HttpContext?.RequestServices.GetService<ICookieMgrService>());

        private IProfileAppService profileAppService;
        public IProfileAppService ProfileAppService => profileAppService ?? (profileAppService = HttpContext?.RequestServices.GetService<IProfileAppService>());

        private IUserPreferencesAppService userPreferencesAppService;
        public IUserPreferencesAppService UserPreferencesAppService => userPreferencesAppService ?? (userPreferencesAppService = HttpContext?.RequestServices.GetService<IUserPreferencesAppService>());

        public Guid CurrentUserId { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            if (User != null && User.Identity.IsAuthenticated && ViewBag.Username == null)
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                CurrentUserId = new Guid(userId);

                var userIsAdmin = User.FindFirst(x => x.Type == ClaimTypes.Role && x.Value.Equals(Roles.Administrator.ToString()));

                if (userIsAdmin != null)
                {
                    ViewData["user_is_admin"] = "true";
                }

                string username = User.FindFirstValue(ClaimTypes.Name);

                SetProfileOnSession(CurrentUserId, username);

                ViewBag.CurrentUserId = CurrentUserId;
                ViewBag.Username = username ?? Constants.DefaultUsername;
                ViewBag.ProfileImage = UrlFormatter.ProfileImage(CurrentUserId);
                ViewBag.Locale = GetAspNetCultureCookie();
            }
        }

        protected void SetProfileOnSession(Guid userId, string userName)
        {
            string sessionUserName = GetSessionValue(SessionValues.Username);

            if (sessionUserName != null && !sessionUserName.Equals(userName))
            {
                SetSessionValue(SessionValues.Username, userName);
            }

            string sessionFullName = GetSessionValue(SessionValues.FullName);

            if (sessionFullName == null)
            {
                ProfileViewModel profile = ProfileAppService.GetByUserId(userId, ProfileType.Personal);
                if (profile != null)
                {
                    SetSessionValue(SessionValues.FullName, profile.Name);
                }
            }
        }

        protected void SetEmailConfirmed(ApplicationUser user)
        {
            string sessionEmailConfirmed = user.EmailConfirmed.ToString();

            SetEmailConfirmed(sessionEmailConfirmed);
        }

        protected void SetEmailConfirmed()
        {
            string sessionEmailConfirmed = GetSessionValue(SessionValues.EmailConfirmed);
            if (string.IsNullOrWhiteSpace(sessionEmailConfirmed))
            {
                Task<ApplicationUser> task = UserManager.GetUserAsync(User);
                task.Wait();

                ApplicationUser user = task.Result;

                sessionEmailConfirmed = user?.EmailConfirmed.ToString() ?? "False";
            }

            SetEmailConfirmed(sessionEmailConfirmed);
        }

        private void SetEmailConfirmed(string sessionEmailConfirmed)
        {
            SetSessionValue(SessionValues.EmailConfirmed, sessionEmailConfirmed);

            ViewData["emailConfirmed"] = sessionEmailConfirmed;
        }

        protected string GetAvatar()
        {
            return GetCookieValue(SessionValues.UserProfileImageUrl);
        }

        protected void SetAvatar(string profileImageUrl)
        {
            SetCookieValue(SessionValues.UserProfileImageUrl, profileImageUrl, 7, true);
        }

        protected ProfileViewModel SetAuthorDetails(UserGeneratedBaseViewModel vm)
        {
            if (vm == null)
            {
                return null;
            }

            if (vm.Id == Guid.Empty || vm.UserId == Guid.Empty)
            {
                vm.UserId = CurrentUserId;
            }

            ProfileViewModel profile = ProfileAppService.GetUserProfileWithCache(vm.UserId);

            if (profile != null)
            {
                vm.AuthorName = profile.Name;
                vm.AuthorPicture = UrlFormatter.ProfileImage(vm.UserId);
            }

            return profile;
        }

        protected SupportedLanguage SetLanguageFromCulture(string languageCode)
        {
            switch (languageCode)
            {
                case "pt-BR":
                case "pt":
                    return SupportedLanguage.Portuguese;

                case "ru":
                case "ru-RU":
                    return SupportedLanguage.Russian;

                case "de":
                    return SupportedLanguage.German;

                case "es":
                    return SupportedLanguage.Spanish;

                case "bs":
                    return SupportedLanguage.Bosnian;

                case "sr":
                    return SupportedLanguage.Serbian;

                case "hr":
                    return SupportedLanguage.Croatian;

                default:
                    return SupportedLanguage.English;
            }
        }

        protected void SetAspNetCultureCookie(SupportedLanguage language)
        {
            string culture = language.GetAttributeOfType<UiInfoAttribute>()?.Culture;

            culture = string.IsNullOrWhiteSpace(culture) ? "en-US" : culture;

            SetAspNetCultureCookie(new RequestCulture(culture));
        }

        protected void SetAspNetCultureCookie(RequestCulture culture)
        {
            ViewBag.Locale = culture;

            SetCookieValue(CookieRequestCultureProvider.DefaultCookieName, CookieRequestCultureProvider.MakeCookieValue(culture), 365);
        }

        protected string GetAspNetCultureCookie()
        {
            var cookieValue = GetCookieValue(CookieRequestCultureProvider.DefaultCookieName);

            var cookie = CookieRequestCultureProvider.ParseCookieValue(cookieValue);

            if (cookie == null || !cookie.Cultures.Any())
            {
                return "en-US";
            }

            return cookie.Cultures.First().Value;
        }

        #region Upload Management

        #region Main Methods

        private string UploadImage(Guid userId, string imageType, string filename, byte[] fileBytes, params string[] tags)
        {
            Task<string> op = ImageStorageService.StoreImageAsync(userId.ToString(), imageType.ToLower() + "_" + filename, fileBytes, tags);
            op.Wait();

            if (!op.IsCompletedSuccessfully)
            {
                throw op.Exception;
            }

            string url = op.Result;

            return url;
        }

        private string DeleteImage(Guid userId, string filename)
        {
            Task<string> op = ImageStorageService.DeleteImageAsync(userId.ToString(), filename);
            op.Wait();

            if (!op.IsCompletedSuccessfully)
            {
                throw op.Exception;
            }

            string url = op.Result;

            return url;
        }

        private string DeleteImage(Guid userId, string imageType, string filename)
        {
            Task<string> op = ImageStorageService.DeleteImageAsync(userId.ToString(), imageType.ToLower() + "_" + filename);
            op.Wait();

            if (!op.IsCompletedSuccessfully)
            {
                throw op.Exception;
            }

            string url = op.Result;

            return url;
        }

        #endregion Main Methods

        protected string UploadImage(Guid userId, ImageType container, string filename, byte[] fileBytes, params string[] tags)
        {
            string containerName = container.ToString().ToLower();

            return UploadImage(userId, containerName, filename, fileBytes, tags);
        }

        protected string UploadGameImage(Guid userId, ImageType type, string filename, byte[] fileBytes, params string[] tags)
        {
            string result = UploadImage(userId, type.ToString().ToLower(), filename, fileBytes, tags);

            return result;
        }

        protected string UploadContentImage(Guid userId, string filename, byte[] fileBytes, params string[] tags)
        {
            string type = ImageType.ContentImage.ToString().ToLower();
            string result = UploadImage(userId, type, filename, fileBytes, tags);

            return result;
        }

        protected string UploadFeaturedImage(Guid userId, string filename, byte[] fileBytes, params string[] tags)
        {
            string type = ImageType.FeaturedImage.ToString().ToLower();
            string result = UploadImage(userId, type, filename, fileBytes, tags);

            return result;
        }

        protected string DeleteGameImage(Guid userId, ImageType type, string filename)
        {
            string result = DeleteImage(userId, filename);

            return result;
        }

        protected string DeleteFeaturedImage(Guid userId, string filename)
        {
            string result = DeleteImage(userId, filename);

            return result;
        }

        #endregion Upload Management

        #region Cookie Management

        protected string GetCookieValue(SessionValues key)
        {
            string value = CookieMgrService.Get(key.ToString());

            return value;
        }

        private string GetCookieValue(string key)
        {
            string value = CookieMgrService.Get(key);

            return value;
        }

        protected void SetCookieValue(SessionValues key, string value, int? expireTime)
        {
            SetCookieValue(key, value, expireTime, false);
        }

        protected void SetCookieValue(SessionValues key, string value, int? expireTime, bool isEssential)
        {
            CookieMgrService.Set(key.ToString(), value, expireTime, isEssential);
        }

        protected void SetCookieValue(string key, string value, int? expireTime)
        {
            SetCookieValue(key, value, expireTime, false);
        }

        protected void SetCookieValue(string key, string value, int? expireTime, bool isEssential)
        {
            CookieMgrService.Set(key, value, expireTime, isEssential);
        }

        #endregion Cookie Management
    }
}