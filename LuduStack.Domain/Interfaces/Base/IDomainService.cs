using System;
using System.Collections.Generic;

namespace LuduStack.Domain.Interfaces
{
    public interface IDomainService<T>
    {
        Guid Add(T model);

        Guid Update(T model);

        void Remove(Guid id);
    }
}