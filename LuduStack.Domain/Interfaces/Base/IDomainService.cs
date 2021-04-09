using System;

namespace LuduStack.Domain.Interfaces
{
    public interface IDomainService<T>
    {
        void Remove(Guid id);
    }
}