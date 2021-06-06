using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.ForumCategory
{
    public class GetForumCategoryByIdQuery : GetByIdBaseQuery<Models.ForumCategory>
    {
        public string Handler { get; }

        public GetForumCategoryByIdQuery(Guid id) : base(id)
        {
        }

        public GetForumCategoryByIdQuery(Guid id, string handler) : base(id)
        {
            Handler = handler;
        }
    }

    public class GetForumCategoryByIdQueryHandler : GetByIdBaseQueryHandler<GetForumCategoryByIdQuery, Models.ForumCategory, IForumCategoryRepository>
    {
        public GetForumCategoryByIdQueryHandler(IForumCategoryRepository repository) : base(repository)
        {
        }

        public override async Task<Models.ForumCategory> Handle(GetForumCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            Models.ForumCategory obj;

            if (!string.IsNullOrWhiteSpace(request.Handler))
            {
                obj = repository.Get(x => x.Handler.Equals(request.Handler)).FirstOrDefault();
            }
            else
            {
                obj = await repository.GetById(request.Id);
            }

            if (obj != null)
            {
                obj.CreateDate = obj.CreateDate.ToLocalTime();

                obj.PublishDate = obj.PublishDate.ToLocalTime();
            }

            return obj;
        }
    }
}