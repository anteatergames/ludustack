using System.Threading.Tasks;

namespace LuduStack.Infra.CrossCutting.Messaging
{
    public interface IMediatorHandler
    {
        Task<TResult> Query<T, TResult>(T query) where T : Query<TResult>;

        Task<CommandResult> SendCommand<T>(T command) where T : Command;

        Task<CommandResult<TResult>> SendCommand<T, TResult>(T command) where T : Command<TResult>;
    }
}