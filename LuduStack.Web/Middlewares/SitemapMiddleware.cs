using LuduStack.Application.Interfaces;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.ValueObjects;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LuduStack.Web.Middlewares
{
    public class SitemapMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _rootUrl;

        private readonly List<string> forbiddenAreas;

        private readonly List<KeyValuePair<string, string>> forbidden;

        public IGameAppService GameAppService { get; private set; }

        public IProfileAppService ProfileAppService { get; private set; }

        public IUserContentAppService ContentAppService { get; private set; }

        public SitemapMiddleware(RequestDelegate next, string rootUrl)
        {
            _next = next;
            _rootUrl = rootUrl;
            forbiddenAreas = new List<string>
            {
                "member",
                "staff"
            };

            forbidden = new List<KeyValuePair<string, string>>
            {
                { "routes", "*" },
                { "storage", "*" },
                { "user", "*" },
                { "test", "*" },
                { "go", "*" },

                { "*", "new" },
                { "*", "add" },
                { "*", "help" },

                { "*", "edit" },
                { "*", "delete" },
                { "*", "save" },

                { "account", "lockout" },
                { "account", "externallogin" },
                { "account", "resetpassword" },
                { "account", "forgotpasswordconfirmation" },
                { "account", "resetpasswordconfirmation" },
                { "account", "accessdenied" },

                { "home", "error" },
                { "home", "counters" },
                { "home", "notifications" },
                { "home", "errortest" },
                { "home", "gameidea" },

                { "user", "listprofiles" },

                { "manage", "resetauthenticatorwarning" },
                { "manage", "showrecoverycodes" },
                { "manage", "error" },
                { "manage", "showrecoverycodes" },

                { "game", "latest" },
                { "game", "list" },
                { "game", "byteam" },

                { "content", "feed" },

                { "brainstorm", "details" },
                { "brainstorm", "list" },
                { "brainstorm", "newsession" },
                { "brainstorm", "newidea" },

                { "userbadge", "list" },

                { "help", "index" },
                { "search", "searchposts" },

                { "team", "details" },
                { "team", "list" },
                { "team", "listbyuser" },
                { "team", "acceptinvitation" },
                { "team", "rejectinvitation" },
                { "team", "deleteteam" },
                { "team", "removemember" },
                { "team", "listmyteams" },

                { "userbadge", "listbyuser" },

                { "jobposition", "details" },
                { "jobposition", "list" },
                { "jobposition", "listmine" },
                { "jobposition", "mypositionsstats" },
                { "jobposition", "myapplications" },

                { "localization", "details" },
                { "localization", "list" },
                { "localization", "listmine" },
                { "localization", "translate" },
                { "localization", "export" },
                { "localization", "exportxml" },
                { "localization", "exportcontributors" },
                { "localization", "review" },
                { "localization", "getterms" },

                { "course", "list" },
                { "course", "listbyme" },
                { "course", "listmine" },
                { "course", "addcourse" },
                { "course", "savecourse" },
                { "course", "details" },
                { "course", "listplans" },
                { "course", "listplansforedit" },
                { "course", "edit" },
                { "study", "listmymentors" },
                { "study", "listmystudents" },

                { "giveaway", "savegiveaway" },
                { "giveaway", "listbyme" },
                { "giveaway", "details" },
                { "giveaway", "youarein" },
                { "giveaway", "manage" },
                { "giveaway", "emailconfirmation" },
                { "giveaway", "terms" },
            };
        }

        public async Task Invoke(HttpContext context, IGameAppService gameAppService, IProfileAppService profileAppService, IUserContentAppService contentAppService)
        {
            StringBuilder sb = new StringBuilder();

            GameAppService = gameAppService;
            ProfileAppService = profileAppService;
            ContentAppService = contentAppService;

            if (context.Request.Path.Value.Equals("/sitemap.xml", StringComparison.OrdinalIgnoreCase))
            {
                Stream stream = context.Response.Body;
                context.Response.StatusCode = 200;
                context.Response.ContentType = "application/xml";
                sb.Append("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

                Assembly assembly = Assembly.GetExecutingAssembly();

                List<Type> controllers = assembly.GetTypes()
                    .Where(type => typeof(Controller).IsAssignableFrom(type)
                    || type.Name.EndsWith("controller")).ToList();

                foreach (Type controller in controllers)
                {
                    sb.AppendLine(await CheckController(controller));
                }

                sb.AppendLine("</urlset>");
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(sb.ToString());
                    memoryStream.Write(bytes, 0, bytes.Length);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    await memoryStream.CopyToAsync(stream, bytes.Length);
                }
            }
            else
            {
                await _next(context);
            }
        }

        private async Task<string> CheckController(Type controller)
        {
            StringBuilder sb = new StringBuilder();

            IEnumerable<MethodInfo> methods = controller.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                                    .Where(method => typeof(IActionResult).IsAssignableFrom(method.ReturnType));

            foreach (MethodInfo method in methods)
            {
                sb.AppendLine(CheckMethod(controller, method));
            }

            List<string> detailMethods = await CheckDetailsMethod(controller);

            foreach (string method in detailMethods)
            {
                sb.AppendLine(method);
            }

            return sb.ToString();
        }

        private string CheckMethod(Type controller, MethodInfo method)
        {
            bool isPost = method.CustomAttributes.Any(x => x.AttributeType == typeof(HttpPostAttribute));
            bool isDelete = method.CustomAttributes.Any(x => x.AttributeType == typeof(HttpDeleteAttribute));
            bool isPut = method.CustomAttributes.Any(x => x.AttributeType == typeof(HttpPutAttribute));

            RouteAttribute routeAttribute = method.GetCustomAttributes<RouteAttribute>().FirstOrDefault();

            bool hasParameter = routeAttribute != null && !routeAttribute.Template.Contains("{");
            string routeTemplate = routeAttribute != null ? routeAttribute.Template.Trim('/') : string.Empty;

            string actionName = method.Name.ToLower();

            return CheckMethod(controller, actionName, isPost, isDelete, isPut, hasParameter, routeTemplate);
        }

        private string CheckMethod(Type controller, string actionName, bool isPost, bool isDelete, bool isPut, bool hasParameter, string routeTemplate)
        {
            string sitemapContent = string.Empty;

            string controllerName = controller.Name.ToLower().Replace("controller", "");

            bool isForbidden = forbidden.Contains(new KeyValuePair<string, string>(controllerName, actionName));
            isForbidden = isForbidden || forbidden.Contains(new KeyValuePair<string, string>(controllerName, "*"));
            isForbidden = isForbidden || forbidden.Contains(new KeyValuePair<string, string>("*", actionName));
            bool areaForbidden = forbiddenAreas.Any(x => controller.Namespace.ToLower().Contains(".areas." + x));

            if (!isPost && !isDelete && !isPut && !areaForbidden && !isForbidden)
            {
                sitemapContent = FillUrl(actionName, hasParameter, routeTemplate, sitemapContent, controllerName);
            }

            return sitemapContent;
        }

        private string FillUrl(string actionName, bool hasParameter, string routeTemplate, string sitemapContent, string controllerName)
        {
            sitemapContent += "<url>";

            if (hasParameter)
            {
                sitemapContent += string.Format("<loc>{0}/{1}/</loc>", _rootUrl.Trim('/'), routeTemplate);
            }
            else
            {
                string methodName = actionName.Equals("index") ? string.Empty : actionName;
                if (string.IsNullOrWhiteSpace(methodName))
                {
                    if (controllerName.Equals("home") && actionName.Equals("index"))
                    {
                        sitemapContent += string.Format("<loc>{0}/</loc>", _rootUrl.Trim('/'));
                    }
                    else
                    {
                        sitemapContent += string.Format("<loc>{0}/{1}/</loc>", _rootUrl.Trim('/'), controllerName.Trim('/'));
                    }
                }
                else
                {
                    sitemapContent += string.Format("<loc>{0}/{1}/{2}/</loc>", _rootUrl.Trim('/'), controllerName.Trim('/'), methodName.Trim('/'));
                }
            }

            sitemapContent += string.Format("<lastmod>{0}</lastmod>", DateTime.UtcNow.ToString("yyyy-MM-dd"));
            sitemapContent += "</url>";
            return sitemapContent;
        }

        private async Task<List<string>> CheckDetailsMethod(Type controller)
        {
            List<string> methodList = new List<string>();

            if (controller.Name.Equals("ProfileController"))
            {
                OperationResultListVo<string> handlersResult = await ProfileAppService.GetAllHandlers(Guid.Empty);

                if (handlersResult != null && handlersResult.Success && handlersResult.Value.Any())
                {
                    List<string> urls = GetDetailUrls(controller, handlersResult.Value, "u/{0}");

                    methodList.AddRange(urls);
                }
            }
            else if (controller.Name.Equals("GameController"))
            {
                OperationResultListVo<Guid> result = await GameAppService.GetAllIds(Guid.Empty);

                if (result != null && result.Success && result.Value.Any())
                {
                    IEnumerable<string> stringIds = result.Value.Select(x => x.ToString());

                    List<string> urls = GetDetailUrls(controller, stringIds, "game/{0}");
                    methodList.AddRange(urls);
                }
            }
            else if (controller.Name.Equals("ContentController") || controller.Name.Equals("ComicsController"))
            {
                OperationResultListVo<UserContentIdAndTypeVo> allContent = await ContentAppService.GetAllContentIds();

                if (allContent != null && allContent.Success && allContent.Value.Any())
                {
                    IEnumerable<UserContentIdAndTypeVo> allPosts = allContent.Value.Where(x => x.Type == UserContentType.Post);
                    IEnumerable<UserContentIdAndTypeVo> allComics = allContent.Value.Where(x => x.Type == UserContentType.ComicStrip);

                    IEnumerable<string> allPostIds = allPosts.Select(x => x.Id.ToString());
                    IEnumerable<string> allComicsIds = allComics.Select(x => x.Id.ToString());

                    List<string> postUrls = GetDetailUrls(controller, allPostIds, "content/{0}");
                    methodList.AddRange(postUrls);

                    List<string> comicsUrls = GetDetailUrls(controller, allComicsIds, "comics/{0}");
                    methodList.AddRange(comicsUrls);
                }
            }

            return methodList;
        }

        private List<string> GetDetailUrls(Type controller, IEnumerable<string> result, string patternUrl)
        {
            List<string> methodList = new List<string>();

            foreach (string handler in result)
            {
                string route = string.Format(patternUrl, handler);

                string sitemapItem = CheckMethod(controller, "details", false, false, false, true, route);

                methodList.Add(sitemapItem);
            }

            return methodList;
        }
    }

    public static class BuilderExtensions
    {
        public static IApplicationBuilder UseSitemapMiddleware(this IApplicationBuilder app)
        {
            return UseSitemapMiddleware(app, "https://www.ludustack.com");
        }

        public static IApplicationBuilder UseSitemapMiddleware(this IApplicationBuilder app, string rootUrl)
        {
            return app.UseMiddleware<SitemapMiddleware>(new[] { rootUrl });
        }
    }

    public static class ListExtension
    {
        public static void Add(this List<KeyValuePair<string, string>> list, string key, string value)
        {
            list.Add(new KeyValuePair<string, string>(key, value));
        }
    }
}