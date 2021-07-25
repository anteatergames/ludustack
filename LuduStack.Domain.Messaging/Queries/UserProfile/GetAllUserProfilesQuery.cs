using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.UserProfile
{
    public class GetAllUserProfilesQuery : Query<IEnumerable<Models.UserProfile>>
    {
    }

    public class GetAllUserProfilesQueryHandler : QueryHandler, IRequestHandler<GetAllUserProfilesQuery, IEnumerable<Models.UserProfile>>
    {
        private readonly IUserProfileRepository userProfileRepository;

        public GetAllUserProfilesQueryHandler(IUserProfileRepository userProfileRepository)
        {
            this.userProfileRepository = userProfileRepository;
        }

        public async Task<IEnumerable<Models.UserProfile>> Handle(GetAllUserProfilesQuery request, CancellationToken cancellationToken)
        {
            return await userProfileRepository.GetAll();
        }
    }
}