using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.GameJam
{
    public class GetGameJamByIdQuery : GetByIdBaseQuery<Models.GameJam>
    {
        public string Handler { get; }

        public GetGameJamByIdQuery(Guid id) : base(id)
        {
        }

        public GetGameJamByIdQuery(Guid id, string handler) : this(id)
        {
            Handler = handler;
        }
    }

    public class GetGameJamByIdQueryHandler : GetByIdBaseQueryHandler<GetGameJamByIdQuery, Models.GameJam, IGameJamRepository>
    {
        public GetGameJamByIdQueryHandler(IGameJamRepository repository) : base(repository)
        {
        }

        public override async Task<Models.GameJam> Handle(GetGameJamByIdQuery request, CancellationToken cancellationToken)
        {
            Models.GameJam obj;

            if (!string.IsNullOrWhiteSpace(request.Handler))
            {
                var query = repository.Get(x => x.Handler.Equals(request.Handler));

                obj = query.FirstOrDefault();
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