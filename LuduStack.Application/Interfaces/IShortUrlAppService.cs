using LuduStack.Domain.ValueObjects;
using System.Threading.Tasks;

namespace LuduStack.Application.Interfaces
{
    public interface IShortUrlAppService
    {
        Task<OperationResultVo> GetFullUrlByToken(string token);
    }
}