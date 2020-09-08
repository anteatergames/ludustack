using FluentValidation.Results;
using System.Threading.Tasks;

namespace LuduStack.Infra.CrossCutting.Messaging
{
    public interface IMediatorHandler
    {
        Task<ValidationResult> SendCommand<T>(T command) where T : Command;
    }
}
