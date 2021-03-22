using LuduStack.Web.ViewComponents.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LuduStack.Web.ViewComponents
{
    public class GameIdeaViewComponent : BaseViewComponent
    {
        public GameIdeaViewComponent(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
        }

        public Task<IViewComponentResult> InvokeAsync()
        {
            return Task.FromResult((IViewComponentResult)View());
        }
    }
}