using LuduStack.Application.ViewModels.Content;
using LuduStack.Application.ViewModels.FeaturedContent;
using LuduStack.Application.ViewModels.Home;
using LuduStack.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LuduStack.Application.Interfaces
{
    public interface IFeaturedContentAppService : ICrudAppService<FeaturedContentViewModel>
    {
        CarouselViewModel GetFeaturedNow();

        Task<OperationResultVo<Guid>> Add(Guid userId, Guid contentId, string title, string introduction);

        IEnumerable<UserContentToBeFeaturedViewModel> GetContentToBeFeatured();

        Task<OperationResultVo> Unfeature(Guid id);
    }
}