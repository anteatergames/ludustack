using LuduStack.Domain.ValueObjects;
using LuduStack.Web.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;
using System;
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

            ViewBag.BaseUrl = GetBaseUrl();
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

        private void TranslateResponse(OperationResultVo response)
        {
            if (response != null && !String.IsNullOrWhiteSpace(response.Message))
            {
                response.Message = SharedLocalizer[response.Message];
            }
        }

        private void TranslateResponse(UploadResultVo response)
        {
            if (response != null && !String.IsNullOrWhiteSpace(response.Message))
            {
                response.Message = SharedLocalizer[response.Message];
            }
        }

        protected void SetGamificationMessage(int? pointsEarned)
        {
            if (pointsEarned.HasValue && pointsEarned.Value > 0)
            {
                TempData["Message"] = SharedLocalizer["You earned {0} points. Awesome!", pointsEarned].Value;
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
    }
}