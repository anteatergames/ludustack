using AutoMapper;
using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Infrastructure;
using LuduStack.Infra.CrossCutting.Messaging;

namespace LuduStack.Application.Interfaces
{
    public interface IBaseAppServiceCommon
    {
        IMapper Mapper { get; }

        IUnitOfWork UnitOfWork { get; }

        IMediatorHandler Mediator { get; }

        ICacheService CacheService { get; }
    }
}