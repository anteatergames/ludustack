using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.UserProfile
{
    public class GetBasicUserProfileDataByUserIdQuery : Query<UserProfileEssentialVo>
    {
        public Guid UserId { get; }

        public GetBasicUserProfileDataByUserIdQuery(Guid userId)
        {
            UserId = userId;
        }
    }

    public class GetBasicUserProfileDataByUserIdQueryHandler : QueryHandler, IRequestHandler<GetBasicUserProfileDataByUserIdQuery, UserProfileEssentialVo>
    {
        private readonly IUserProfileRepository userProfileRepository;

        public GetBasicUserProfileDataByUserIdQueryHandler(IUserProfileRepository userProfileRepository)
        {
            this.userProfileRepository = userProfileRepository;
        }

        public async Task<UserProfileEssentialVo> Handle(GetBasicUserProfileDataByUserIdQuery request, CancellationToken cancellationToken)
        {
            UserProfileEssentialVo profile = await userProfileRepository.GetBasicDataByUserId(request.UserId);

            return profile;
        }
    }
}