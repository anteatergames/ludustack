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
        private readonly IFeaturedContentRepository featuredContentRepository;

        public UserContentDomainService(IUserContentRepository repository, IFeaturedContentRepository featuredContentRepository) : base(repository)
        {
            this.featuredContentRepository = featuredContentRepository;
        }

        public new IEnumerable<UserContentSearchVo> Search(Expression<Func<UserContent, bool>> where)
        {
            IEnumerable<UserContent> all = base.Search(where);

            IEnumerable<UserContentSearchVo> selected = all.OrderByDescending(x => x.CreateDate)
                .Select(x => new UserContentSearchVo
                {
                    ContentId = x.Id,
                    Title = x.Title,
                    FeaturedImage = x.FeaturedImage,
                    Content = (string.IsNullOrWhiteSpace(x.Introduction) ? x.Content : x.Introduction).GetFirstWords(20),
                    Language = (x.Language == 0 ? SupportedLanguage.English : x.Language)
                });

            return selected;
        }

        public void AddLike(UserContentLike model)
        {
            Task.Run(async () => await repository.AddLike(model));
        }

        public bool CheckIfCommentExists<T>(Expression<Func<UserContentComment, bool>> where)
        {
            Task<IQueryable<UserContentComment>> task = repository.GetComments(where);

            task.Wait();

            bool exists = task.Result.Any();

            return exists;
        }

        public void Comment(UserContentComment model)
        {
            Task.Run(async () => await repository.AddComment(model));
        }

        public int CountComments(Expression<Func<UserContentComment, bool>> where)
        {
            Task<int> countTask = repository.CountComments(where);

            countTask.Wait();

            return countTask.Result;
        }

        public int CountCommentsByUserId(Guid userId)
        {
            Task<int> countTask = repository.CountComments(x => x.UserId == userId);

            countTask.Wait();

            return countTask.Result;
        }

        public IQueryable<UserContent> GetActivityFeed(Guid? gameId, Guid? userId, List<SupportedLanguage> languages, Guid? oldestId, DateTime? oldestDate, bool? articlesOnly, int count)
        {
            IQueryable<UserContent> allModels = repository.Get();

            List<Guid> featuredIds = featuredContentRepository.Get(x => x.Active).Select(x => x.UserContentId).ToList();

            if (featuredIds.Any())
            {
                allModels = allModels.Where(x => !featuredIds.Contains(x.Id));
            }

            if (articlesOnly.HasValue && articlesOnly.Value)
            {
                allModels = allModels.Where(x => !string.IsNullOrEmpty(x.Title) && !string.IsNullOrEmpty(x.Introduction) && !string.IsNullOrEmpty(x.FeaturedImage) && x.Content.Length > 50);
            }

            if (userId.HasValue && userId != Guid.Empty)
            {
                allModels = allModels.Where(x => x.UserId != Guid.Empty && x.UserId == userId);
            }

            if (gameId.HasValue && gameId != Guid.Empty)
            {
                allModels = allModels.Where(x => x.GameId != Guid.Empty && x.GameId == gameId);
            }

            if (languages != null && languages.Any())
            {
                allModels = allModels.Where(x => x.Language == 0 || languages.Contains(x.Language));
            }

            if (oldestDate.HasValue)
            {
                allModels = allModels.Where(x => x.CreateDate <= oldestDate && x.Id != oldestId);
            }

            IOrderedQueryable<UserContent> orderedList = allModels
                .OrderByDescending(x => x.CreateDate);

            IQueryable<UserContent> finalList = orderedList.Take(count);

            return finalList;
        }

        public IQueryable<UserContentComment> GetComments(Expression<Func<UserContentComment, bool>> where)
        {
            Task<IQueryable<UserContentComment>> task = Task.Run(async () => await repository.GetComments(where));

            return task.Result;
        }

        public IEnumerable<UserContentLike> GetLikes(Func<UserContentLike, bool> where)
        {
            Task<IQueryable<UserContentLike>> task = Task.Run(async () => await repository.GetLikes(where));

            return task.Result;
        }

        public void RemoveLike(Guid currentUserId, Guid userContentId)
        {
            Task.Run(async () => await repository.RemoveLike(currentUserId, userContentId));
        }

        #region Comics
        public List<ComicsListItemVo> GetComicsListByUserId(Guid currentUserId)
        {
            var allModels = repository.Get().Where(x => x.UserId == currentUserId && x.UserContentType == UserContentType.ComicStrip)
                .Select(x => new ComicsListItemVo
                {
                    Id = x.Id,
                    IssueNumber = x.IssueNumber.HasValue ? x.IssueNumber.Value : 0,
                    Title = x.Title,
                    Content = x.Content,
                    FeaturedImage = x.FeaturedImage,
                    CreateDate = x.CreateDate
                });


            return allModels.ToList();
        }

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