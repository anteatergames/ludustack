using LuduStack.Domain.ValueObjects;
using LuduStack.Web.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LuduStack.Web.Controllers.Base
{
    public class BaseController : Controller
    {
        private IStringLocalizer<SharedResources> _sharedLocalizer;
        public IStringLocalizer<SharedResources> SharedLocalizer => _sharedLocalizer ?? (_sharedLocalizer = (IStringLocalizer<SharedResources>)HttpContext?.RequestServices.GetService(typeof(IStringLocalizer<SharedResources>)));

        protected JsonSerializerOptions DefaultJsonSerializeOptions { get; }

        public BaseController()
        {
            DefaultJsonSerializeOptions = new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            ViewData["BaseUrl"] = GetBaseUrl();
        }

        protected string GetBaseUrl()
        {
            string hostUrl = WebUtility.UrlDecode($"{Request.Scheme}://{Request.Host}{Request.PathBase}");

            ViewData["protocol"] = Request.IsHttps ? "https" : "http";
            ViewData["host"] = Request.Host.ToString();

            return hostUrl;
        }

        protected string GetSessionValue(SessionValues key)
        {
            string value = HttpContext.Session.GetString(key.ToString());

            return value;
        }

        protected void SetSessionValue(SessionValues key, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                HttpContext.Session.SetString(key.ToString(), value);
            }
        }

        protected static string GetFileExtension(IFormFile uploadedFile)
        {
            string extension = GetFileExtension(uploadedFile.FileName);

            return extension;
        }

        protected static string GetFileExtension(string fileUrl)
        {
            string[] split = fileUrl.Split('.');
            string extension = split.Length > 1 ? split[1] : "jpg";
            return extension;
        }

        protected JsonResult Json(OperationResultVo operationVo)
        {
            if (!string.IsNullOrWhiteSpace(operationVo.Message))
            {
                operationVo.Message = SharedLocalizer[operationVo.Message];
            }

            return Json((object)operationVo);
        }

        protected IActionResult RedirectToWithMessage(string action, string controller, string area, string message)
        {
            return RedirectToAction(action, controller, new { area, msg = SharedLocalizer[message] });
        }
    }
}