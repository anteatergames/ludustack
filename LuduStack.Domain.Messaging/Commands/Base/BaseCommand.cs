using LuduStack.Infra.CrossCutting.Messaging;
using System;

namespace LuduStack.Domain.Messaging
{
    public abstract class BaseCommand : Command
    {
        public Guid Id { get; internal set; }

        public BaseCommand(Guid id)
        {
            Id = id;
            AggregateId = id;
        }

        public virtual bool IsValid()
        {
            return Result.Validation.IsValid;
        }
    }
}