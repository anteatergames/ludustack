using LuduStack.Application;
using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Store;
using LuduStack.Domain.ValueObjects;
using LuduStack.Web.Areas.Staff.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Web.Areas.Staff.Controllers
{
    [Route("staff/product")]
    public class ProductController : StaffBaseController
    {
        private readonly IProductAppService productAppService;

        public ProductController(IProductAppService productAppService)
        {
            this.productAppService = productAppService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("list")]
        public async Task<PartialViewResult> List()
        {
            ProductListViewModel model;

            OperationResultVo serviceResult = await productAppService.Get(CurrentUserId);

            if (serviceResult.Success)
            {
                OperationResultVo<ProductListViewModel> castResult = serviceResult as OperationResultVo<ProductListViewModel>;

                model = castResult.Value;
            }
            else
            {
                model = new ProductListViewModel();
            }

            SetPermissions(model.Elements);

            model.Elements = model.Elements.OrderBy(x => x.CreateDate).ToList();

            return PartialView("_ListProducts", model);
        }

        [Route("add")]
        public IActionResult Add()
        {
            ProductViewModel model = new ProductViewModel();

            return View("CreateEditWrapper", model);
        }

        [Route("edit/{id:guid}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            ProductViewModel viewModel;

            OperationResultVo serviceResult = await productAppService.GetById(CurrentUserId, id);

            OperationResultVo<ProductViewModel> castResult = serviceResult as OperationResultVo<ProductViewModel>;

            viewModel = castResult.Value;

            SetPermissions(viewModel);

            return View("CreateEditWrapper", viewModel);
        }

        [HttpPost("save")]
        public async Task<JsonResult> Save(ProductViewModel vm)
        {
            bool isNew = vm.Id == Guid.Empty;

            try
            {
                vm.UserId = CurrentUserId;

                OperationResultVo<Guid> saveResult = await productAppService.Save(CurrentUserId, vm);

                if (saveResult.Success)
                {
                    string url = Url.Action("index", "product", new { area = "staff", msg = SharedLocalizer[saveResult.Message] });

                    if (isNew && EnvName.Equals(Constants.ProductionEnvironmentName))
                    {
                        await NotificationSender.SendTeamNotificationAsync("New Product created!");
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
                OperationResultVo saveResult = await productAppService.Sync(CurrentUserId);

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
                OperationResultVo deleteResult = await productAppService.Remove(CurrentUserId, id);

                if (deleteResult.Success)
                {
                    if (edit)
                    {
                        string url = Url.Action("index", "product", new { area = "staff", msg = deleteResult.Message });
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

        private void SetPermissions(List<ProductViewModel> list)
        {
            foreach (ProductViewModel product in list)
            {
                SetPermissions(product);
            }
        }

        private void SetPermissions(ProductViewModel viewModel)
        {
            viewModel.Permissions.IsAdmin = CurrentUserIsAdmin;
            viewModel.Permissions.CanDelete = viewModel.Permissions.IsAdmin;
        }
    }
}