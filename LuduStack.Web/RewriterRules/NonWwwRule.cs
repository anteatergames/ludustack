using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;

namespace LuduStack.Web.RewriterRules
{
    public class NonWwwRule : IRule
    {
        public void ApplyRule(RewriteContext context)
        {
            HostString host = context.HttpContext.Request.Host;

            if (host.HasValue && host.Value.ToLower().Contains(".ludustack.com") || context.HttpContext.Request.Path.Value.EndsWith("assetlinks.json"))
            {
                context.Result = RuleResult.SkipRemainingRules;
            }
            else
            {
                context.Result = RuleResult.ContinueRules;
            }
        }
    }
}