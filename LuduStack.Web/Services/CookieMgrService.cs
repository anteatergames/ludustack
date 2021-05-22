using Microsoft.AspNetCore.Http;
using System;

namespace LuduStack.Web.Services
{
    public class CookieMgrService : ICookieMgrService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CookieMgrService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string Get(string key)
        {
            string cookieValueFromContext = _httpContextAccessor.HttpContext.Request.Cookies[key];

            return cookieValueFromContext;
        }

        public void Set(string key, string value, int? expireTime, bool isEssential, SameSiteMode? sameSite)
        {
            string consent = _httpContextAccessor.HttpContext.Request.Cookies[".LuduStack.Consent"];

            CookieOptions option = new CookieOptions
            {
                HttpOnly = true
            };

            if (expireTime.HasValue)
            {
                option.Expires = DateTime.Now.AddDays(expireTime.Value);
            }
            else
            {
                option.Expires = DateTime.Now.AddDays(7);
            }

            option.IsEssential = isEssential || consent != null;
            option.Secure = true;
            option.SameSite = sameSite.HasValue ? sameSite.Value : SameSiteMode.Strict;

            string existing = _httpContextAccessor.HttpContext.Request.Cookies[key];

            if (existing != null)
            {
                _httpContextAccessor.HttpContext.Response.Cookies.Delete(key);
            }

            _httpContextAccessor.HttpContext.Response.Cookies.Append(key, value, option);
        }
    }
}