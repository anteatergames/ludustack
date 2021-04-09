using LuduStack.Domain.Core.Models;
using LuduStack.Domain.Interfaces;
using System;

namespace LuduStack.Domain.Services
{
    public abstract class BaseDomainMongoService<T, TRepository> : IDomainService<T> where T : Entity where TRepository : class, IRepository<T>
    {
        protected readonly TRepository repository;

        protected BaseDomainMongoService(TRepository repository)
        {
            this.repository = repository;
        }

        public virtual void Remove(Guid id)
        {
            repository.Remove(id);
        }
    }
}