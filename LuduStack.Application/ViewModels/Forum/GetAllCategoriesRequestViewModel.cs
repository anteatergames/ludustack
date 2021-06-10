using LuduStack.Domain.Core.Enums;
using System.Collections.Generic;

namespace LuduStack.Application.ViewModels.Forum
{
    public class GetAllCategoriesRequestViewModel
    {
        public List<SupportedLanguage> Languages { get; set; }
    }
}