using LuduStack.Application.ViewModels.ShortUrl;
using LuduStack.Domain.ValueObjects;

namespace LuduStack.Application.Interfaces
{
    public interface IShortUrlAppService : ICrudAppService<ShortUrlViewModel>
    {
        OperationResultVo GetFullUrlByToken(string token);
    }
}