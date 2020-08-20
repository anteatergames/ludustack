using LuduStack.Application.Formatters;
using LuduStack.Application.Helpers;
using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels;
using LuduStack.Application.ViewModels.Comics;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuduStack.Application.Services
{
    public class ComicsAppService : ProfileBaseAppService, IComicsAppService
    {
        private readonly IUserContentDomainService userContentDomainService;

        public ComicsAppService(IProfileBaseAppServiceCommon profileBaseAppServiceCommon, IUserContentDomainService userContentDomainService) : base(profileBaseAppServiceCommon)
        {
            this.userContentDomainService = userContentDomainService;
        }

        #region ICrudAppService

        public OperationResultVo<int> Count(Guid currentUserId)
        {
            throw new NotImplementedException();
        }

        public OperationResultListVo<ComicStripViewModel> GetAll(Guid currentUserId)
        {
            throw new NotImplementedException();
        }

        public OperationResultVo GetAllIds(Guid currentUserId)
        {
            throw new NotImplementedException();
        }

        public OperationResultVo<ComicStripViewModel> GetById(Guid currentUserId, Guid id)
        {
            throw new NotImplementedException();
        }

        public OperationResultVo Remove(Guid currentUserId, Guid id)
        {
            try
            {
                // validate before

                userContentDomainService.Remove(id);

                unitOfWork.Commit();

                return new OperationResultVo(true, "That Comic Strip is gone now!");
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public OperationResultVo<Guid> Save(Guid currentUserId, ComicStripViewModel viewModel)
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

                unitOfWork.Commit();

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
                //int lastIssueNumber = userContentDomainService.GetMaxIssueNumber(currentUserId);

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

        public OperationResultVo GetComicsByMe(Guid currentUserId)
        {
            try
            {
                List<ComicsListItemVo> comics = userContentDomainService.GetComicsListByUserId(currentUserId);

                return new OperationResultListVo<ComicsListItemVo>(comics);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public OperationResultVo GetForDetails(Guid currentUserId, Guid id)
        {
            try
            {
                UserContent existing = userContentDomainService.GetById(id);

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

        public OperationResultVo GetForEdit(Guid currentUserId, Guid id)
        {
            try
            {
                UserContent existing = userContentDomainService.GetById(id);

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

        public OperationResultVo Rate(Guid currentUserId, Guid id, decimal scoreDecimal)
        {
            try
            {
                DomainOperationVo<UserContentRating> domainActionPerformed = userContentDomainService.Rate(currentUserId, id, scoreDecimal);

                unitOfWork.Commit();

                if (domainActionPerformed.Action == DomainActionPerformed.Update)
                {
                    return new OperationResultVo(true, "Your rate was updated!");
                }
                else
                {
                    return new OperationResultVo(true, "Comics rated!");
                }
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