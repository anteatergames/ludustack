using MediatR;
using System.Threading.Tasks;

namespace LuduStack.Infra.CrossCutting.Messaging
{
    public sealed class InMemoryBus : IMediatorHandler
    {
        private readonly IMediator mediator;

        public InMemoryBus(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task<TResult> Query<T, TResult>(T query) where T : Query<TResult>
        {
            return await mediator.Send<TResult>(query);
        }

        public async Task<CommandResult> SendCommand<T>(T command) where T : Command
        {
            return await mediator.Send<CommandResult>(command);
        }

        public async Task<CommandResult<TResult>> SendCommand<T, TResult>(T command) where T : Command<TResult>
        {
            return await mediator.Send<CommandResult<TResult>>(command);
        }
    }
}