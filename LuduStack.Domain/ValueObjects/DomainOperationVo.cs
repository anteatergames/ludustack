using LuduStack.Domain.Core.Enums;

namespace LuduStack.Domain.ValueObjects
{
    public class DomainOperationVo<T>
    {
        public DomainActionPerformed Action { get; private set; }

        public T Entity { get; private set; }

        public DomainOperationVo(DomainActionPerformed action, T entity)
        {
            this.Action = action;
            this.Entity = entity;
        }
    }
}