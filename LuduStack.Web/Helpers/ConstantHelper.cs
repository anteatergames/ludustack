using LuduStack.Application;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Text.Json;

namespace LuduStack.Web
{
    public static class ConstantHelper
    {
        public static string SiteName { get { return "LUDUSTACK"; } }

        public static string DefaultTitle { get { return "LUDUSTACK - The one stop place for game developers"; } }

        public static string DefaultDescription { get { return "The one stop place for game developers, artists and musicians. Helping making gamedev dreams come true."; } }

        public static string DefaultAvatar { get { return Constants.DefaultAvatar; } }

        public static IEnumerable<SelectListItem> TimeZones
        {
            get
            {
                return JsonSerializer.Deserialize<IEnumerable<SelectListItem>>(Constants.TimeZones);
            }
        }
    }
}