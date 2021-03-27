using Microsoft.AspNetCore.Http;

namespace LuduStack.Web.Services
{
    public interface ICookieMgrService
    {
        string Get(string key);

        void Set(string key, string value, int? expireTime, bool isEssential, SameSiteMode? sameSite);
    }
}