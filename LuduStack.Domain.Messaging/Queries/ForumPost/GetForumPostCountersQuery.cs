using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.ForumCategory
{
    public class GetForumPostCountersQuery : Query<IEnumerable<ForumPostCounterResultVo>>
    {
        public Guid? ForumCategoryId { get; }

        public GetForumPostCountersQuery()
        {
        }

        public GetForumPostCountersQuery(Guid? forumCategoryId)
        {
            ForumCategoryId = forumCategoryId;
        }
    }

    public class GetForumPostCountersQueryHandler : QueryHandler, IRequestHandler<GetForumPostCountersQuery, IEnumerable<ForumPostCounterResultVo>>
    {
        private readonly IForumPostRepository forumPostRepository;

        public GetForumPostCountersQueryHandler(IForumPostRepository forumPostRepository)
        {
            this.forumPostRepository = forumPostRepository;
        }

        public async Task<IEnumerable<ForumPostCounterResultVo>> Handle(GetForumPostCountersQuery request, CancellationToken cancellationToken)
        {
            List<ForumPostConterDataVo> rawData = await forumPostRepository.GetForumPostCounterInformation(request.ForumCategoryId);

            IEnumerable<IGrouping<Guid, ForumPostConterDataVo>> grouped = rawData.GroupBy(x => x.OriginalPostId);

            IEnumerable<ForumPostCounterResultVo> result = grouped.Select(x => new ForumPostCounterResultVo
            {
                OriginalPostId = x.Key,
                ReplyCount = x.Count(y => !y.IsOriginalPost),
                ViewCount = x.Sum(y => y.Views),
                LatestReply = x.Where(x => !x.IsOriginalPost).OrderByDescending(x => x.CreateDate).Select(x => new LatestForumPostResultVo
                {
                    Id = x.Id,
                    OriginalPostId = x.OriginalPostId,
                    UserId = x.UserId,
                    CreateDate = x.CreateDate,
                    Title = x.Title
                }).FirstOrDefault()
            });

            return result;
        }
    }
}