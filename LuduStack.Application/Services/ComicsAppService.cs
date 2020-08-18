using LuduStack.Application.Formatters;
using LuduStack.Application.Helpers;
using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Comics;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc.Routing;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

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

        public OperationResultVo<Guid> Save(Guid currentUserId, ComicStripViewModel vm)
        {
            int pointsEarned = 0;

            try
            {
                UserContent model;

                UserContent existing = userContentDomainService.GetById(vm.Id);
                if (existing != null)
                {
                    model = mapper.Map(vm, existing);
                }
                else
                {
                    model = mapper.Map<UserContent>(vm);
                }

                FormatImagesToSave(model);

                if (vm.Id == Guid.Empty)
                {
                    userContentDomainService.Add(model);
                    vm.Id = model.Id;

                    //pointsEarned += gamificationDomainService.ProcessAction(currentUserId, PlatformAction.ComicsAdd);
                }
                else
                {
                    userContentDomainService.Update(model);
                }

                unitOfWork.Commit();

                vm.Id = model.Id;

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

                var currentUserRate = existing.Ratings.FirstOrDefault(x => x.UserId == currentUserId);

                if (currentUserRate != null)
                {
                    vm.CurrentUserRating = currentUserRate.Score;
                }

                var ratingCounts = existing.Ratings.Count > 0 ? existing.Ratings.Count : 1;

                vm.TotalRating = existing.Ratings.Sum(x => x.Score) / ratingCounts;

                SetAuthorDetails(vm);

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

                return new OperationResultVo(true, "Comics rated!");
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
            var except = model.Images.Where(x => x.Image.Contains(Constants.DefaultComicStripPlaceholder) || Constants.DefaultComicStripPlaceholder.Contains(x.Image));
            model.Images = model.Images.Except(except).ToList();
        }

        private void SetImagesToShow(ComicStripViewModel vm, bool editing)
        {
            vm.FeaturedImage = ContentHelper.SetFeaturedImage(vm.UserId, vm.FeaturedImage, ImageRenderType.Full);
            vm.FeaturedImageLquip = ContentHelper.SetFeaturedImage(vm.UserId, vm.FeaturedImage, ImageRenderType.LowQuality);

            foreach (var image in vm.Images)
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
                var selectedFeaturedImage = vm.FeaturedImage;

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
    }
}
