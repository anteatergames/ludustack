using MediatR;
using System;

namespace LuduStack.Infra.CrossCutting.Messaging
{
    public abstract class Command : Message, IRequest<CommandResult>
    {
        public DateTime Timestamp { get; private set; }

        public CommandResult Result { get; set; }

        protected Command()
        {
            Timestamp = DateTime.Now;
            Result = new CommandResult();
        }
    }

    public abstract class Command<T> : Message, IRequest<CommandResult<T>>
    {
        public DateTime Timestamp { get; private set; }

        public CommandResult<T> Result { get; set; }

        protected Command()
        {
            Timestamp = DateTime.Now;
            Result = new CommandResult<T>();
        }

        protected Command(T result)
        {
            Timestamp = DateTime.Now;
            Result = new CommandResult<T>(result);
        }
    }
}