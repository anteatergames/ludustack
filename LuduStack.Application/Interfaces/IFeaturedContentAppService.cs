using LuduStack.Application.ViewModels.Content;
using LuduStack.Application.ViewModels.Home;
using LuduStack.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LuduStack.Application.Interfaces
{
    public interface IFeaturedContentAppService
    {
        Task<CarouselViewModel> GetFeaturedNow();

        Task<OperationResultVo<Guid>> Add(Guid userId, Guid contentId, string title, string introduction);

        Task<IEnumerable<UserContentToBeFeaturedViewModel>> GetContentToBeFeatured();

        Task<OperationResultVo> Unfeature(Guid userId, Guid id);
    }
}