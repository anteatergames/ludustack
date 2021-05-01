using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.UserContent
{
    public class GetUserContentIdsAndTypesQuery : Query<IEnumerable<UserContentIdAndTypeVo>>
    {
        public Expression<Func<Models.UserContent, bool>> Where { get; set; }

        public GetUserContentIdsAndTypesQuery()
        {
        }

        public GetUserContentIdsAndTypesQuery(Expression<Func<Models.UserContent, bool>> where)
        {
            Where = where;
        }
    }

    public class GetUserContentIdsAndTypesQueryHandler : QueryHandler, IRequestHandler<GetUserContentIdsAndTypesQuery, IEnumerable<UserContentIdAndTypeVo>>
    {
        private readonly IUserContentRepository repository;

        public GetUserContentIdsAndTypesQueryHandler(IUserContentRepository repository)
        {
            this.repository = repository;
        }

        public Task<IEnumerable<UserContentIdAndTypeVo>> Handle(GetUserContentIdsAndTypesQuery request, CancellationToken cancellationToken)
        {
            if (request.Where != null)
            {
                return Task.FromResult(repository.Get(request.Where).Select(x => new UserContentIdAndTypeVo
                {
                    Id = x.Id,
                    Type = x.UserContentType
                }).AsEnumerable());
            }
            else
            {
                return Task.FromResult(repository.Get().Select(x => new UserContentIdAndTypeVo
                {
                    Id = x.Id,
                    Type = x.UserContentType
                }).AsEnumerable());
            }
        }
    }
}