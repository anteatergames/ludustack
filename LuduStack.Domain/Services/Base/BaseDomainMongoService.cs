using LuduStack.Domain.Core.Models;
using LuduStack.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public virtual IEnumerable<T> GetAll()
        {
            IEnumerable<T> objs = repository.GetAll().Result;

            return objs.ToList();
        }

        public virtual IEnumerable<Guid> GetAllIds()
        {
            IEnumerable<Guid> objs = repository.Get().Select(x => x.Id);

            return objs.ToList();
        }

        public virtual T GetById(Guid id)
        {
            T obj = repository.GetById(id).Result;

            if (obj != null)
            {
                obj.CreateDate = obj.CreateDate.ToLocalTime();

                obj.PublishDate = obj.PublishDate.ToLocalTime();
            }

            return obj;
        }

        public virtual IEnumerable<T> GetByUserId(Guid userId)
        {
            var task = repository.GetByUserId(userId);

            if (task.Status == TaskStatus.Faulted)
            {
                return new List<T>();
            }

            IEnumerable<T> obj = task.Result;

            return obj.ToList();
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

        IQueryable<T> IDomainService<T>.Search(Expression<Func<T, bool>> where)
        {
            return repository.Get(where);
        }
    }
}