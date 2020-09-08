using FluentValidation.Results;
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

        public async Task<ValidationResult> SendCommand<T>(T command) where T : Command
        {
            return await mediator.Send<ValidationResult>(command);
        }
    }
}
