using System;
using System.Collections.Generic;

namespace LuduStack.Domain.Interfaces
{
    public interface IDomainService<T>
    {
        IEnumerable<T> GetAll();

        IEnumerable<Guid> GetAllIds();

        T GetById(Guid id);

        IEnumerable<T> GetByUserId(Guid userId);

        Guid Add(T model);

        Guid Update(T model);

        void Remove(Guid id);
    }
}