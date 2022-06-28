using LuduStack.Application;
using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Store;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.ValueObjects;
using LuduStack.Web.Areas.Staff.Controllers.Base;
using LuduStack.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Web.Areas.Staff.Controllers
{
    [Route("staff/order")]
    public class OrderController : StaffBaseController
    {
        private readonly IOrderAppService orderAppService;

        public OrderController(IOrderAppService orderAppService)
        {
            this.orderAppService = orderAppService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("list")]
        public async Task<PartialViewResult> List()
        {
            OrderListViewModel model;

            OperationResultVo serviceResult = await orderAppService.Get(CurrentUserId);

            if (serviceResult.Success)
            {
                OperationResultVo<OrderListViewModel> castResult = serviceResult as OperationResultVo<OrderListViewModel>;

                model = castResult.Value;
            }
            else
            {
                model = new OrderListViewModel();
            }

            SetPermissions(model.Elements);

            model.Elements = model.Elements.OrderBy(x => x.CreateDate).ToList();

            return PartialView("_ListOrders", model);
        }

        [Route("add")]
        public IActionResult Add()
        {
            OrderViewModel model = new OrderViewModel();

            return View("CreateEditWrapper", model);
        }

        [Route("edit/{id:guid}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            OrderViewModel viewModel;

            OperationResultVo serviceResult = await orderAppService.GetById(CurrentUserId, id);

            OperationResultVo<OrderViewModel> castResult = serviceResult as OperationResultVo<OrderViewModel>;

            viewModel = castResult.Value;

            SetPermissions(viewModel);

            return View("CreateEditWrapper", viewModel);
        }

        [HttpPost("save")]
        public async Task<JsonResult> Save(OrderViewModel vm)
        {
            bool isNew = vm.Id == Guid.Empty;

            try
            {
                vm.UserId = CurrentUserId;

                OperationResultVo<Guid> saveResult = await orderAppService.Save(CurrentUserId, vm);

                if (saveResult.Success)
                {
                    string url = Url.Action("index", "order", new { area = "staff", msg = SharedLocalizer[saveResult.Message] });

                    if (isNew && EnvName.Equals(Constants.ProductionEnvironmentName))
                    {
                        await NotificationSender.SendTeamNotificationAsync("New Order created!");
                    }

                    return Json(new OperationResultRedirectVo<Guid>(saveResult, url));
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

        [HttpPost("sync")]
        public async Task<JsonResult> Sync()
        {
            try
            {
                OperationResultVo saveResult = await orderAppService.Sync(CurrentUserId);

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

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, bool edit)
        {
            try
            {
                OperationResultVo deleteResult = await orderAppService.Remove(CurrentUserId, id);

                if (deleteResult.Success)
                {
                    if (edit)
                    {
                        string url = Url.Action("index", "order", new { area = "staff", msg = deleteResult.Message });
                        deleteResult.Message = null;

                        return Json(new OperationResultRedirectVo(deleteResult, url));
                    }

                    return Json(deleteResult);
                }
                else
                {
                    return Json(new OperationResultVo(false));
                }
            }
            catch (Exception ex)
            {
                return Json(new OperationResultVo(ex.Message));
            }
        }

        private void SetPermissions(List<OrderViewModel> list)
        {
            foreach (OrderViewModel order in list)
            {
                SetPermissions(order);
            }
        }

        private void SetPermissions(OrderViewModel viewModel)
        {
            viewModel.Permissions.IsAdmin = CurrentUserIsAdmin;
            viewModel.Permissions.CanDelete = viewModel.Permissions.IsAdmin;
        }
    }
}