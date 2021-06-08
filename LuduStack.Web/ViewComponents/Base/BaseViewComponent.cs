using LuduStack.Application.Interfaces;
using LuduStack.Domain.Core.Enums;
using LuduStack.Web.Enums;
using LuduStack.Web.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace LuduStack.Web.ViewComponents.Base
{
    public abstract class BaseViewComponent : ViewComponent
    {
        private IServiceProvider services;
        public IServiceProvider Services => services ??= HttpContext?.RequestServices.GetService<IServiceProvider>();

        private IStringLocalizer<SharedResources> _sharedLocalizer;
        public IStringLocalizer<SharedResources> SharedLocalizer => _sharedLocalizer ?? (_sharedLocalizer = (IStringLocalizer<SharedResources>)HttpContext?.RequestServices.GetService(typeof(IStringLocalizer<SharedResources>)));

        private IUserPreferencesAppService userPreferencesAppService;
        public IUserPreferencesAppService UserPreferencesAppService => userPreferencesAppService ??= Services.GetService<IUserPreferencesAppService>();

        private ICookieMgrService _cookieMgrService;
        public ICookieMgrService CookieMgrService => _cookieMgrService ??= Services.GetService<ICookieMgrService>();

        private IMediator mediator;
        public IMediator Mediator => mediator ?? (mediator = (IMediator)HttpContext?.RequestServices.GetService(typeof(IMediator)));

        public Guid CurrentUserId { get; set; }

        public bool CurrentUserIsAdmin { get; set; }

        protected BaseViewComponent(IHttpContextAccessor httpContextAccessor)
        {
            ClaimsPrincipal authenticatedUser = httpContextAccessor.HttpContext.User;

            if (authenticatedUser != null)
            {
                string id = authenticatedUser.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!string.IsNullOrWhiteSpace(id))
                {
                    CurrentUserId = new Guid(id);

                    CurrentUserIsAdmin = authenticatedUser.Identity.IsAuthenticated && authenticatedUser.IsInRole(Roles.Administrator.ToString());
                }
            }
        }

        protected async Task<List<SupportedLanguage>> GetCurrentUserContentLanguage()
        {
            string languagesFromCookie = CookieMgrService.Get(SessionValues.ContentLanguages.ToString());
            if (string.IsNullOrWhiteSpace(languagesFromCookie))
            {
                return await UserPreferencesAppService.GetLanguagesByUserId(CurrentUserId);
            }
            else
            {
                return JsonSerializer.Deserialize<List<SupportedLanguage>>(languagesFromCookie);
            }
        }
    }
}