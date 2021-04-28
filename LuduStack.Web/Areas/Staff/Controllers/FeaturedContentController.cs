using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Content;
using LuduStack.Domain.ValueObjects;
using LuduStack.Web.Areas.Staff.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LuduStack.Web.Areas.Staff.Controllers
{
    [Route("staff/featuredcontent")]
    public class FeaturedContentController : StaffBaseController
    {
        private readonly IFeaturedContentAppService featuredContentAppService;

        public FeaturedContentController(IFeaturedContentAppService service)
        {
            featuredContentAppService = service;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("list")]
        public async Task<IActionResult> List()
        {
            OperationResultVo<FeaturedContentScreenViewModel> model = await featuredContentAppService.GetContentToBeFeatured();

            return PartialView("_List", model.Value);
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add(Guid id, string title, string introduction)
        {
            OperationResultVo<Guid> operationResult = await featuredContentAppService.Add(CurrentUserId, id, title, introduction);

            return Json(operationResult);
        }

        [HttpPost("remove")]
        public async Task<IActionResult> Remove(Guid id, string title, string introduction)
        {
            OperationResultVo operationResult;

            try
            {
                operationResult = await featuredContentAppService.Unfeature(CurrentUserId, id);
            }
            catch (Exception ex)
            {
                operationResult = new OperationResultVo(ex.Message);
            }

            return Json(operationResult);
        }
    }
}