using LuduStack.Application.Formatters;
using LuduStack.Application.Helpers;
using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels;
using LuduStack.Application.ViewModels.Comics;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Messaging;
using LuduStack.Domain.Messaging.Queries.UserContent;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Application.Services
{
    public class ComicsAppService : ProfileBaseAppService, IComicsAppService
    {
        private readonly IUserContentDomainService userContentDomainService;

        public ComicsAppService(IMediatorHandler mediator, IProfileBaseAppServiceCommon profileBaseAppServiceCommon, IUserContentDomainService userContentDomainService) : base(mediator, profileBaseAppServiceCommon)
        {
            this.userContentDomainService = userContentDomainService;
        }

        #region ICrudAppService

        public Task<OperationResultVo<int>> Count(Guid currentUserId)
        {
            return Task.FromResult(new OperationResultVo<int>("Not Implemented"));
        }

        public Task<OperationResultListVo<ComicStripViewModel>> GetAll(Guid currentUserId)
        {
            return Task.FromResult(new OperationResultListVo<ComicStripViewModel>("Not Implemented"));
        }

        public Task<OperationResultVo> GetAllIds(Guid currentUserId)
        {
            return Task.FromResult(new OperationResultVo("Not Implemented"));
        }

        public Task<OperationResultVo<ComicStripViewModel>> GetById(Guid currentUserId, Guid id)
        {
            return Task.FromResult(new OperationResultVo<ComicStripViewModel>("Not Implemented"));
        }

        public async Task<OperationResultVo> Remove(Guid currentUserId, Guid id)
        {
            try
            {
                CommandResult result = await mediator.SendCommand(new DeleteUserContentCommand(id));

                if (!result.Validation.IsValid)
                {
                    return new OperationResultVo(result.Validation.Errors.First().ErrorMessage);
                }
                else
                {
                    return new OperationResultVo(true, "That Comic Strip is gone now!");
                }
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public async Task<OperationResultVo<Guid>> Save(Guid currentUserId, ComicStripViewModel viewModel)
        {
            int pointsEarned = 0;

            try
            {
                UserContent model;

                UserContent existing = userContentDomainService.GetById(viewModel.Id);
                if (existing != null)
                {
                    model = mapper.Map(viewModel, existing);
                }
                else
                {
                    model = mapper.Map<UserContent>(viewModel);
                }

                FormatImagesToSave(model);

                if (viewModel.Id == Guid.Empty)
                {
                    userContentDomainService.Add(model);
                    viewModel.Id = model.Id;
                }
                else
                {
                    userContentDomainService.Update(model);
                }

                await unitOfWork.Commit();

                viewModel.Id = model.Id;

                return new OperationResultVo<Guid>(model.Id, pointsEarned);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<Guid>(ex.Message);
            }
        }

        #endregion ICrudAppService

        public OperationResultVo GenerateNew(Guid currentUserId)
        {
            try
            {
                ComicStripViewModel newVm = new ComicStripViewModel
                {
                    UserId = currentUserId,
                    SeriesId = currentUserId,
                    PublishDate = DateTime.Now,
                    Language = SupportedLanguage.English
                };

                newVm.Images.Add(new ImageListItemVo
                {
                    Language = newVm.Language,
                    Image = Constants.DefaultComicStripPlaceholder
                });

                newVm.Images.Add(new ImageListItemVo
                {
                    Language = SupportedLanguage.Portuguese,
                    Image = Constants.DefaultComicStripPlaceholder
                });

                return new OperationResultVo<ComicStripViewModel>(newVm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public async Task<OperationResultVo> GetComicsByMe(Guid currentUserId)
        {
            try
            {
                var comics = await mediator.Query<GetComicsByUserIdQuery, IEnumerable<UserContent>>(new GetComicsByUserIdQuery(currentUserId));

                var voList = comics.Select(x => new ComicsListItemVo
                {
                    Id = x.Id,
                    IssueNumber = x.IssueNumber.HasValue ? x.IssueNumber.Value : 0,
                    Title = x.Title,
                    Content = x.Content,
                    FeaturedImage = x.FeaturedImage,
                    CreateDate = x.CreateDate
                });

                return new OperationResultListVo<ComicsListItemVo>(voList);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public async Task<OperationResultVo> GetForDetails(Guid currentUserId, Guid id)
        {
            try
            {
                UserContent existing = await mediator.Query<GetUserContentByIdQuery, UserContent>(new GetUserContentByIdQuery(id));

                ComicStripViewModel vm = mapper.Map<ComicStripViewModel>(existing);

                UserContentRating currentUserRate = existing.Ratings.FirstOrDefault(x => x.UserId == currentUserId);

                if (currentUserRate != null)
                {
                    vm.CurrentUserRating = currentUserRate.Score;
                }

                int ratingCounts = existing.Ratings.Count > 0 ? existing.Ratings.Count : 1;

                vm.RatingCount = existing.Ratings.Count;
                vm.TotalRating = existing.Ratings.Sum(x => x.Score) / ratingCounts;

                vm.LikeCount = vm.Likes.Count;
                vm.CommentCount = vm.Comments.Count;

                SetAuthorDetails(vm);

                LoadAuthenticatedData(currentUserId, vm);

                SetImagesToShow(vm, false);

                return new OperationResultVo<ComicStripViewModel>(vm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public async Task<OperationResultVo> GetForEdit(Guid currentUserId, Guid id)
        {
            try
            {
                UserContent existing = await mediator.Query<GetUserContentByIdQuery, UserContent>(new GetUserContentByIdQuery(id));

                ComicStripViewModel vm = mapper.Map<ComicStripViewModel>(existing);

                SetAuthorDetails(vm);

                SetImagesToShow(vm, true);

                SetPermissions(currentUserId, vm);

                return new OperationResultVo<ComicStripViewModel>(vm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public async Task<OperationResultVo> Rate(Guid currentUserId, Guid id, decimal scoreDecimal)
        {
            try
            {
                await mediator.SendCommand(new RateUserContentCommand(currentUserId, id, scoreDecimal));

                return new OperationResultVo(true, "Your rate has been registered!");
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        private void SetPermissions(Guid currentUserId, ComicStripViewModel vm)
        {
            SetBasePermissions(currentUserId, vm);
        }

        private void FormatImagesToSave(UserContent model)
        {
            IEnumerable<ImageListItemVo> except = model.Images.Where(x => x.Image.Contains(Constants.DefaultComicStripPlaceholder) || Constants.DefaultComicStripPlaceholder.Contains(x.Image));
            model.Images = model.Images.Except(except).ToList();
        }

        private void SetImagesToShow(ComicStripViewModel vm, bool editing)
        {
            vm.FeaturedImage = ContentHelper.SetFeaturedImage(vm.UserId, vm.FeaturedImage, ImageRenderType.Full);
            vm.FeaturedImageLquip = ContentHelper.SetFeaturedImage(vm.UserId, vm.FeaturedImage, ImageRenderType.LowQuality);

            foreach (ImageListItemVo image in vm.Images)
            {
                image.Image = ContentHelper.SetFeaturedImage(vm.UserId, image.Image, ImageRenderType.Full);
                image.ImageResponsive = ContentHelper.SetFeaturedImage(vm.UserId, image.Image, ImageRenderType.Responsive);
                image.ImageLquip = ContentHelper.SetFeaturedImage(vm.UserId, image.Image, ImageRenderType.LowQuality);
            }

            if (editing)
            {
                if (!vm.Images.Any(x => x.Language == SupportedLanguage.English))
                {
                    vm.Images.Add(new ImageListItemVo
                    {
                        Language = SupportedLanguage.English,
                        Image = Constants.DefaultComicStripPlaceholder
                    });
                }

                if (!vm.Images.Any(x => x.Language == SupportedLanguage.Portuguese))
                {
                    vm.Images.Add(new ImageListItemVo
                    {
                        Language = SupportedLanguage.Portuguese,
                        Image = Constants.DefaultComicStripPlaceholder
                    });
                }
            }
            else
            {
                string selectedFeaturedImage = vm.FeaturedImage;

                if (vm.Images.Any(x => x.Language == vm.Language))
                {
                    selectedFeaturedImage = vm.Images.FirstOrDefault(x => x.Language == vm.Language)?.Image;
                }
                else if (vm.Images.Any())
                {
                    selectedFeaturedImage = vm.Images.FirstOrDefault()?.Image;
                }

                vm.FeaturedImage = ContentHelper.SetFeaturedImage(vm.UserId, selectedFeaturedImage, ImageRenderType.Full);
                vm.FeaturedImageResponsive = ContentHelper.SetFeaturedImage(vm.UserId, selectedFeaturedImage, ImageRenderType.Responsive);
                vm.FeaturedImageLquip = ContentHelper.SetFeaturedImage(vm.UserId, selectedFeaturedImage, ImageRenderType.LowQuality);
            }

            vm.Images = vm.Images.OrderBy(x => x.Language).ToList();
        }

        private void LoadAuthenticatedData(Guid currentUserId, UserGeneratedCommentBaseViewModel item)
        {
            if (currentUserId != Guid.Empty)
            {
                item.CurrentUserLiked = item.Likes.Any(x => x == currentUserId);

                foreach (CommentViewModel comment in item.Comments)
                {
                    UserProfile commenterProfile = GetCachedProfileByUserId(comment.UserId);
                    if (commenterProfile == null)
                    {
                        comment.AuthorName = Constants.UnknownSoul;
                    }
                    else
                    {
                        comment.AuthorName = commenterProfile.Name;
                    }

                    comment.AuthorPicture = UrlFormatter.ProfileImage(comment.UserId);
                    comment.Text = string.IsNullOrWhiteSpace(comment.Text) ? Constants.SoundOfSilence : comment.Text;
                }
            }
        }
    }
}