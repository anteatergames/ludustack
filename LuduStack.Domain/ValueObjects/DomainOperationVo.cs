using LuduStack.Domain.Core.Enums;

namespace LuduStack.Domain.ValueObjects
{
    public class DomainOperationVo
    {
        public DomainActionPerformed Action { get; private set; }

        public DomainOperationVo(DomainActionPerformed action)
        {
            Action = action;
        }
    }

    public class DomainOperationVo<T> : DomainOperationVo
    {
        public T Entity { get; private set; }

        public DomainOperationVo(DomainActionPerformed action, T entity) : base(action)
        {
            Entity = entity;
        }
    }
}