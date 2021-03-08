using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Extensions;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LuduStack.Domain.Services
{
    public class UserContentDomainService : BaseDomainMongoService<UserContent, IUserContentRepository>, IUserContentDomainService
    {
        public UserContentDomainService(IUserContentRepository repository) : base(repository)
        {
        }

        public void Comment(UserContentComment model)
        {
            Task.Run(async () => await repository.AddComment(model));
        }

        #region Comics

        public DomainOperationVo<UserContentRating> Rate(Guid userId, Guid id, decimal scoreDecimal)
        {
            UserContentRating rating;

            IQueryable<UserContentRating> existing = repository.GetRatings(id);
            bool exists = existing.Any(x => x.UserId == userId);

            if (exists)
            {
                rating = existing.First(x => x.UserId == userId);

                rating.Score = scoreDecimal;

                repository.UpdateRating(id, rating);

                return new DomainOperationVo<UserContentRating>(DomainActionPerformed.Update, rating);
            }
            else
            {
                rating = new UserContentRating
                {
                    UserId = userId,
                    Score = scoreDecimal
                };

                repository.AddRating(id, rating);

                return new DomainOperationVo<UserContentRating>(DomainActionPerformed.Create, rating);
            }
        }

        #endregion Comics
    }
}