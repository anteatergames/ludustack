using AutoMapper;
using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Infrastructure;
using LuduStack.Domain.Interfaces.Services;

namespace LuduStack.Application.Interfaces
{
    public interface IBaseAppServiceCommon
    {
        IMapper Mapper { get; }

        IUnitOfWork UnitOfWork { get; }

        ICacheService CacheService { get; }
    }
}