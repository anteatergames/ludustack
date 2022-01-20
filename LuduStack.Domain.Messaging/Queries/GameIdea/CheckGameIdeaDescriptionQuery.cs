using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.GameIdea
{
    public class CheckGameIdeaDescriptionQuery : Query<bool>
    {
        public string Description { get; }

        public Guid Id { get; }

        public SupportedLanguage Language { get; }

        public GameIdeaElementType Type { get; }

        public CheckGameIdeaDescriptionQuery(string description, Guid id, Core.Enums.SupportedLanguage language, GameIdeaElementType type)
        {
            Description = description;
            Id = id;
            Language = language;
            Type = type;
        }
    }

    public class CheckGameIdeaDescriptionQueryHandler : QueryHandler, IRequestHandler<CheckGameIdeaDescriptionQuery, bool>
    {
        private readonly IGameIdeaRepository gameIdeaRepository;

        public CheckGameIdeaDescriptionQueryHandler(IGameIdeaRepository gameIdeaRepository)
        {
            this.gameIdeaRepository = gameIdeaRepository;
        }

        public Task<bool> Handle(CheckGameIdeaDescriptionQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Models.GameIdea> exists = gameIdeaRepository.Get(x => x.Description.Equals(request.Description) && x.Language == request.Language && x.Type == request.Type && x.Id != request.Id);

            return Task.FromResult(!exists.Any());
        }
    }
}