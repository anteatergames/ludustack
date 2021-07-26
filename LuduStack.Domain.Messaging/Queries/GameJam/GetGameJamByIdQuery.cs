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

        public GetGameJamByIdQuery(string handler) : this(Guid.Empty)
        {
            Handler = handler;
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
                IQueryable<Models.GameJam> query = repository.Get(x => x.Handler.Equals(request.Handler));

                obj = query.FirstOrDefault();
            }
            else
            {
                obj = await repository.GetById(request.Id);
            }

            if (obj != null)
            {
                SetDates(obj);
            }

            return obj;
        }

        private static void SetDates(Models.GameJam model)
        {
            if (model.StartDate == default)
            {
                model.StartDate = model.CreateDate.AddDays(7);
            }
            if (model.EntryDeadline == default)
            {
                model.EntryDeadline = model.StartDate.AddDays(7);
            }
            if (model.VotingEndDate == default)
            {
                model.VotingEndDate = model.EntryDeadline.AddDays(7);
            }
            if (model.ResultDate == default)
            {
                model.ResultDate = model.VotingEndDate.AddDays(7);
            }
        }
    }
}