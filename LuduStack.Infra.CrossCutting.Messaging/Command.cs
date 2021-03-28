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
}