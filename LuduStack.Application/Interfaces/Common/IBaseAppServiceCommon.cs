using AutoMapper;
using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Infrastructure;

namespace LuduStack.Application.Interfaces
{
    public interface IBaseAppServiceCommon
    {
        IMapper Mapper { get; }

        IUnitOfWork UnitOfWork { get; }

        ICacheService CacheService { get; }
    }
}