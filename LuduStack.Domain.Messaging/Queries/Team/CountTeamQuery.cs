using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;

namespace LuduStack.Domain.Messaging.Queries.Team
{
    public class CountTeamQuery : CountBaseQuery<Models.Team>
    {
        public CountTeamQuery() : base()
        {
        }
    }
    public class CountTeamQueryHandler : CountBaseQueryHandler<CountTeamQuery, Models.Team, ITeamRepository>
    {
        public CountTeamQueryHandler(ITeamRepository repository) : base(repository)
        {
        }
    }
}
