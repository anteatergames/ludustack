using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.UserPreferences
{
    public class GetUserLanguagesByUserIdQuery : Query<IEnumerable<SupportedLanguage>>
    {
        public Guid UserId { get; }

        public GetUserLanguagesByUserIdQuery(Guid userId)
        {
            UserId = userId;
        }
    }

    public class GetUserLanguagesByUserIdQueryHandler : QueryHandler, IRequestHandler<GetUserLanguagesByUserIdQuery, IEnumerable<SupportedLanguage>>
    {
        protected readonly IUserPreferencesRepository repository;

        public GetUserLanguagesByUserIdQueryHandler(IUserPreferencesRepository repository)
        {
            this.repository = repository;
        }

        public virtual async Task<IEnumerable<SupportedLanguage>> Handle(GetUserLanguagesByUserIdQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<SupportedLanguage> languages = await repository.GetUserLanguagesByUserId(request.UserId);

            return languages;
        }
    }
}