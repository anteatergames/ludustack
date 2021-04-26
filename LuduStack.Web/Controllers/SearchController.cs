using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Search;
using LuduStack.Domain.ValueObjects;
using LuduStack.Web.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LuduStack.Web.Controllers
{
    [Route("/search")]
    public class SearchController : SecureBaseController
    {
        private readonly IUserContentAppService userContentAppService;

        public SearchController(IUserContentAppService userContentAppService)
        {
            this.userContentAppService = userContentAppService;
        }

        public IActionResult Index(string q)
        {
            ViewData["term"] = q;
            return View();
        }

        [Route("posts")]
        public async Task<IActionResult> SearchPosts(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return View("_SearchPostsResult", new UserContentSearchViewModel());
            }
            else
            {
                OperationResultListVo<UserContentSearchViewModel> result = await userContentAppService.Search(CurrentUserId, q);
                return View("_SearchPostsResult", result.Value);
            }
        }
    }
}