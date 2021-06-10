using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.ForumGroup
{
    public class GetForumGroupByIdQuery : GetByIdBaseQuery<Models.ForumGroup>
    {
        public GetForumGroupByIdQuery(Guid id) : base(id)
        {
        }
    }

    public class GetForumGroupByIdQueryHandler : GetByIdBaseQueryHandler<GetForumGroupByIdQuery, Models.ForumGroup, IForumGroupRepository>
    {
        public GetForumGroupByIdQueryHandler(IForumGroupRepository repository) : base(repository)
        {
        }

        public override async Task<Models.ForumGroup> Handle(GetForumGroupByIdQuery request, CancellationToken cancellationToken)
        {
            Models.ForumGroup obj = await repository.GetById(request.Id);

            if (obj != null)
            {
                obj.CreateDate = obj.CreateDate.ToLocalTime();

                obj.PublishDate = obj.PublishDate.ToLocalTime();
            }

            return obj;
        }
    }
}