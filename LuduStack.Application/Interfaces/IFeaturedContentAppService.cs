using LuduStack.Application.ViewModels.Content;
using LuduStack.Application.ViewModels.FeaturedContent;
using LuduStack.Application.ViewModels.Home;
using LuduStack.Domain.ValueObjects;
using System;
using System.Collections.Generic;

namespace LuduStack.Application.Interfaces
{
    public interface IFeaturedContentAppService : ICrudAppService<FeaturedContentViewModel>
    {
        CarouselViewModel GetFeaturedNow();

        OperationResultVo<Guid> Add(Guid userId, Guid contentId, string title, string introduction);

        IEnumerable<UserContentToBeFeaturedViewModel> GetContentToBeFeatured();

        OperationResultVo Unfeature(Guid id);
    }
}