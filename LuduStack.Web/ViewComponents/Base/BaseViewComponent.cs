using LuduStack.Domain.Core.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Security.Claims;

namespace LuduStack.Web.ViewComponents.Base
{
    public abstract class BaseViewComponent : ViewComponent
    {
        private IStringLocalizer<SharedResources> _sharedLocalizer;
        public IStringLocalizer<SharedResources> SharedLocalizer => _sharedLocalizer ?? (_sharedLocalizer = (IStringLocalizer<SharedResources>)HttpContext?.RequestServices.GetService(typeof(IStringLocalizer<SharedResources>)));

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
    }
}