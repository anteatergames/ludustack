using LuduStack.Application;
using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.BillRate;
using LuduStack.Application.ViewModels.Store;
using LuduStack.Domain.Core.Attributes;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Extensions;
using LuduStack.Domain.ValueObjects;
using LuduStack.Web.Areas.Member.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Web.Areas.Member.Controllers
{
    [Route("member/store")]
    public class StoreController : MemberBaseController
    {
        private readonly IProductAppService productAppService;
        private readonly IOrderAppService orderAppService;
        private readonly IStorePartnershipAppService storePartnershipAppService;

        public StoreController(IProductAppService productAppService, IOrderAppService orderAppService, IStorePartnershipAppService storePartnershipAppService)
        {
            this.productAppService = productAppService;
            this.orderAppService = orderAppService;
            this.storePartnershipAppService = storePartnershipAppService;
        }

        public IActionResult Dashboard()
        {
            if (User.Identity.IsAuthenticated)
            {
                var model = new StorePartnershipViewModel();

                return View("Dashboard", model);
            }
            else
            {
                return RedirectToAction("index", "home", new { area = string.Empty, msg = SharedLocalizer["You must be authenticated"] });
            }
        }


        [Route("list")]
        public async Task<PartialViewResult> ListMyProducts()
        {
            OperationResultListVo<ProductViewModel> serviceResult = await productAppService.GetByOwner(CurrentUserId, CurrentUserId);

            return PartialView("_ListProducts", serviceResult);
        }


        [Route("mypartnershipdata")]
        public async Task<JsonResult> MyPartnershipData()
        {
            OperationResultVo<StorePartnershipViewModel> serviceResult = await storePartnershipAppService.GetByPartner(CurrentUserId, CurrentUserId);

            return Json(serviceResult);
        }


        [Route("productview/{id:guid}")]
        public async Task<IActionResult> ProductView(Guid id)
        {
            ProductViewModel viewModel;

            OperationResultVo serviceResult = await productAppService.GetById(CurrentUserId, id);

            OperationResultVo<ProductViewModel> castResult = serviceResult as OperationResultVo<ProductViewModel>;

            viewModel = castResult.Value;

            return View("ProductView", viewModel);
        }

        [HttpPost("sync")]
        public async Task<JsonResult> Sync()
        {
            try
            {
                OperationResultVo saveResult = await storePartnershipAppService.Sync(CurrentUserId, CurrentUserId);

                if (saveResult.Success)
                {
                    if (EnvName.Equals(Constants.ProductionEnvironmentName))
                    {
                        await NotificationSender.SendTeamNotificationAsync(saveResult.Message);
                    }

                    return Json(new OperationResultVo(saveResult.Success, saveResult.Message));
                }
                else
                {
                    return Json(saveResult);
                }
            }
            catch (Exception ex)
            {
                return Json(new OperationResultVo(ex.Message));
            }
        }
    }
}