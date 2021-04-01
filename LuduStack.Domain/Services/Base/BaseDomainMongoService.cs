using LuduStack.Domain.Core.Models;
using LuduStack.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Domain.Services
{
    public abstract class BaseDomainMongoService<T, TRepository> : IDomainService<T> where T : Entity where TRepository : class, IRepository<T>
    {
        protected readonly TRepository repository;

        protected BaseDomainMongoService(TRepository repository)
        {
            this.repository = repository;
        }

        public virtual Guid Add(T model)
        {
            repository.Add(model);

            return model.Id;
        }

        public virtual void Remove(Guid id)
        {
            repository.Remove(id);
        }

        public virtual Guid Update(T model)
        {
            repository.Update(model);

            return model.Id;
        }
    }
}