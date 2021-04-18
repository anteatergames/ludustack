using LuduStack.Infra.CrossCutting.Messaging;
using System;

namespace LuduStack.Domain.Messaging
{
    public abstract class BaseCommand : Command
    {
        public Guid Id { get; internal set; }

        protected BaseCommand(Guid id)
        {
            Id = id;
            AggregateId = id;
        }

        public virtual bool IsValid()
        {
            return Result.Validation.IsValid;
        }
    }

    public abstract class BaseCommand<T> : Command<T>
    {
        public Guid Id { get; internal set; }

        protected BaseCommand(Guid id)
        {
            Id = id;
            AggregateId = id;
        }

        public virtual bool IsValid()
        {
            return Result.Validation.IsValid;
        }
    }

    public abstract class BaseUserCommand : BaseCommand
    {
        public Guid UserId { get; internal set; }

        protected BaseUserCommand(Guid userId, Guid id) : base(id)
        {
            UserId = userId;
        }

        protected BaseUserCommand(Guid userId) : base(Guid.Empty)
        {
            UserId = userId;
        }
    }

    public abstract class BaseUserCommand<T> : BaseCommand<T>
    {
        public Guid UserId { get; internal set; }

        protected BaseUserCommand(Guid userId, Guid id) : base(id)
        {
            UserId = userId;
        }
    }
}