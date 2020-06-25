using LuduStack.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace LuduStack.Web.Extensions
{
    public static class IEnumerableExtensions
    {
        public static List<SelectListItem> ToSelectList(this IEnumerable<SelectListItemVo> items)
        {
            List<SelectListItem> finalList = new List<SelectListItem>();

            foreach (SelectListItemVo item in items)
            {
                SelectListItem sli = new SelectListItem
                {
                    Value = item.Value,
                    Text = item.Text
                };

                finalList.Add(sli);
            }

            return finalList;
        }
    }
}