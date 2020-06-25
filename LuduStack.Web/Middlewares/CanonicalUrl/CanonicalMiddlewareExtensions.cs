using Microsoft.AspNetCore.Builder;

namespace LuduStack.Web.Middlewares.CanonicalUrl
{
    public static class CanonicalUrlMiddlewareExtensions
    {
        public static IApplicationBuilder UseCanonicalUrlMiddleware(this IApplicationBuilder builder, CanonicalUrlMiddlewareOptions options)
        {
            return builder.UseMiddleware<CanonicalUrlMiddleware>(options);
        }
    }
}