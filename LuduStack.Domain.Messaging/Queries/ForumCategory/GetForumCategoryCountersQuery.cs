using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.ForumCategory
{
    public class GetForumCategoryCountersQuery : Query<IEnumerable<ForumCategoryCounterResultVo>>
    {
        public List<SupportedLanguage> Languages { get; set; }

        public GetForumCategoryCountersQuery(List<SupportedLanguage> languages)
        {
            Languages = languages;
        }
    }

    public class GetForumCategoryCountersQueryHandler : QueryHandler, IRequestHandler<GetForumCategoryCountersQuery, IEnumerable<ForumCategoryCounterResultVo>>
    {
        private readonly IForumPostRepository forumPostRepository;

        public GetForumCategoryCountersQueryHandler(IForumPostRepository forumPostRepository)
        {
            this.forumPostRepository = forumPostRepository;
        }

        public async Task<IEnumerable<ForumCategoryCounterResultVo>> Handle(GetForumCategoryCountersQuery request, CancellationToken cancellationToken)
        {
            List<ForumCategoryConterDataVo> rawData = await forumPostRepository.GetForumCategoryCounterInformation(request.Languages);

            IEnumerable<IGrouping<System.Guid, ForumCategoryConterDataVo>> grouped = rawData.GroupBy(x => x.ForumCategoryId);

            IEnumerable<ForumCategoryCounterResultVo> result = grouped.Select(x => new ForumCategoryCounterResultVo
            {
                ForumCategoryId = x.Key,
                PostsCount = x.Count(),
                TopicsCount = x.Count(y => y.IsOriginalPost == true),
                LatestPost = x.OrderByDescending(x => x.CreateDate).Select(x => new LatestForumPostResultVo
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    CreateDate = x.CreateDate,
                    Title = x.Title
                }).FirstOrDefault()
            });

            return result;
        }
    }
}