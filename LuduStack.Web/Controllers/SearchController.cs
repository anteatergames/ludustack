using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Search;
using LuduStack.Domain.ValueObjects;
using LuduStack.Web.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult SearchPosts(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return View("_SearchPostsResult", new UserContentSearchViewModel());
            }
            else
            {
                OperationResultListVo<UserContentSearchViewModel> result = userContentAppService.Search(CurrentUserId, q);
                return View("_SearchPostsResult", result.Value);
            }
        }
    }
}