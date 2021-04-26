using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.UserProfile
{
    public class GetBasicUserProfileDataByUserIdsQuery : Query<IEnumerable<UserProfileEssentialVo>>
    {
        public List<Guid> UserIds { get; }

        public GetBasicUserProfileDataByUserIdsQuery(IEnumerable<Guid> userIds)
        {
            UserIds = userIds.ToList();
        }
    }

    public class GetBasicUserProfileDataByUserIdsQueryHandler : QueryHandler, IRequestHandler<GetBasicUserProfileDataByUserIdsQuery, IEnumerable<UserProfileEssentialVo>>
    {
        private readonly IUserProfileRepository userProfileRepository;

        public GetBasicUserProfileDataByUserIdsQueryHandler(IUserProfileRepository userProfileRepository)
        {
            this.userProfileRepository = userProfileRepository;
        }

        public async Task<IEnumerable<UserProfileEssentialVo>> Handle(GetBasicUserProfileDataByUserIdsQuery request, CancellationToken cancellationToken)
        {
            return await userProfileRepository.GetBasicDataByUserIds(request.UserIds);
        }
    }
}