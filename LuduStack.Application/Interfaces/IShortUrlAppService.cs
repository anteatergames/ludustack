using LuduStack.Domain.ValueObjects;

namespace LuduStack.Application.Interfaces
{
    public interface IShortUrlAppService
    {
        OperationResultVo GetFullUrlByToken(string token);
    }
}