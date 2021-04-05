using System;

namespace LuduStack.Domain.Interfaces
{
    public interface IDomainService<T>
    {
        Guid Update(T model);

        void Remove(Guid id);
    }
}