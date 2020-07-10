using LuduStack.Web.Areas.Tools.Controllers;
using LuduStack.Web.Areas.Work.Controllers;
using LuduStack.Web.Controllers;

namespace Microsoft.AspNetCore.Mvc
{
    public static class UrlHelperExtensions
    {
        public static string EmailConfirmationLink(this IUrlHelper urlHelper, string userId, string code, string scheme)
        {
            return urlHelper.Action(
                action: nameof(AccountController.ConfirmEmail),
                controller: "Account",
                values: new { userId, code },
                protocol: scheme);
        }

        public static string ResetPasswordCallbackLink(this IUrlHelper urlHelper, string userId, string code, string scheme)
        {
            return urlHelper.Action(
                action: nameof(AccountController.ResetPassword),
                controller: "Account",
                values: new { userId, code },
                protocol: scheme);
        }

        public static string JobPositionDetailsCallbackLink(this IUrlHelper urlHelper, string id, string scheme)
        {
            return urlHelper.Action(
                action: nameof(JobPositionController.Details),
                controller: "JobPosition",
                values: new { id },
                protocol: scheme);
        }

        public static string GiveawayEmailConfirmationLink(this IUrlHelper urlHelper, string scheme, string id, string referralCode)
        {
            return urlHelper.Action(
                action: nameof(GiveawayController.EmailConfirmation),
                controller: "Giveaway",
                values: new { id, referralCode },
                protocol: scheme);
        }
    }
}