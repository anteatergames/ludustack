using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;

namespace LuduStack.Domain.Messaging.Queries.Team
{
    public class GetTeamByIdQuery : GetByIdBaseQuery<Models.Team>
    {
        public GetTeamByIdQuery(Guid id) : base(id)
        {
        }
    }

    public class GetTeamByIdQueryHandler : GetByIdBaseQueryHandler<GetTeamByIdQuery, Models.Team, ITeamRepository>
    {
        public GetTeamByIdQueryHandler(ITeamRepository repository) : base(repository)
        {
        }
    }
}