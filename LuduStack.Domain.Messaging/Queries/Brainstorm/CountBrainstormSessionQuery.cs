using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;

namespace LuduStack.Domain.Messaging.Queries.Brainstorm
{
    public class CountBrainstormSessionQuery : CountBaseQuery<Models.Game>
    {
    }

    public class CountBrainstormSessionQueryHandler : CountBaseQueryHandler<CountBrainstormSessionQuery, Models.Game, IGameRepository>
    {
        public CountBrainstormSessionQueryHandler(IGameRepository repository) : base(repository)
        {
        }
    }
}