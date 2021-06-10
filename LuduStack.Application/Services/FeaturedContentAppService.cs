using AutoMapper.QueryableExtensions;
using LuduStack.Application.Formatters;
using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Content;
using LuduStack.Application.ViewModels.FeaturedContent;
using LuduStack.Application.ViewModels.Home;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Messaging;
using LuduStack.Domain.Messaging.Queries.FeaturedContent;
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
    public class FeaturedContentAppService : BaseAppService, IFeaturedContentAppService
    {
        public FeaturedContentAppService(IBaseAppServiceCommon baseAppServiceCommon) : base(baseAppServiceCommon)
        {
        }

        public async Task<CarouselViewModel> GetFeaturedNow()
        {
            DateTime now = DateTime.Now;

            IQueryable<FeaturedContent> allModels = (await mediator.Query<GetFeaturedContentQuery, IEnumerable<FeaturedContent>>(new GetFeaturedContentQuery(x => x.StartDate <= now && (!x.EndDate.HasValue || x.EndDate > now)))).AsQueryable();

            if (allModels.Any())
            {
                IEnumerable<FeaturedContentViewModel> vms = allModels.ProjectTo<FeaturedContentViewModel>(mapper.ConfigurationProvider);

                CarouselViewModel model = new CarouselViewModel
                {
                    Items = vms.OrderByDescending(x => x.CreateDate).ToList()
                };

                foreach (FeaturedContentViewModel vm in model.Items)
                {
                    string[] imageSplit = vm.ImageUrl.Split("/");
                    Guid userId = vm.OriginalUserId == Guid.Empty ? vm.UserId : vm.OriginalUserId;

                    vm.FeaturedImage = UrlFormatter.FormatFeaturedImageUrl(userId, imageSplit.Last(), ImageRenderType.Full);
                    vm.FeaturedImageLquip = UrlFormatter.FormatFeaturedImageUrl(userId, imageSplit.Last(), ImageRenderType.LowQuality);
                }

                return model;
            }
            else
            {
                CarouselViewModel fake = new CarouselViewModel();

                return fake;
            }
        }

        public async Task<OperationResultVo<Guid>> Add(Guid userId, Guid contentId, string title, string introduction)
        {
            try
            {
                FeaturedContent newFeaturedContent = new FeaturedContent
                {
                    UserContentId = contentId
                };

                UserContent content = await mediator.Query<GetUserContentByIdQuery, UserContent>(new GetUserContentByIdQuery(contentId));

                newFeaturedContent.Title = string.IsNullOrWhiteSpace(title) ? content.Title : title;
                newFeaturedContent.Introduction = string.IsNullOrWhiteSpace(introduction) ? content.Introduction : introduction;

                newFeaturedContent.ImageUrl = string.IsNullOrWhiteSpace(content.FeaturedImage) || content.FeaturedImage.Equals(Constants.DefaultFeaturedImage) ? Constants.DefaultFeaturedImage : UrlFormatter.Image(content.UserId, ImageType.FeaturedImage, content.FeaturedImage);

                newFeaturedContent.FeaturedImage = content.FeaturedImage;

                newFeaturedContent.StartDate = DateTime.Now;
                newFeaturedContent.Active = true;
                newFeaturedContent.UserId = userId;
                newFeaturedContent.OriginalUserId = content.UserId;

                CommandResult result = await mediator.SendCommand(new SaveFeaturedContentCommand(userId, newFeaturedContent));

                if (!result.Validation.IsValid)
                {
                    string message = result.Validation.Errors.FirstOrDefault().ErrorMessage;
                    return new OperationResultVo<Guid>(newFeaturedContent.Id, false, message);
                }

                return new OperationResultVo<Guid>(newFeaturedContent.Id);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<Guid>(ex.Message);
            }
        }

        public async Task<OperationResultVo<FeaturedContentScreenViewModel>> GetContentToBeFeatured()
        {
            IEnumerable<UserContent> finalList = await mediator.Query<GetUserContentQuery, IEnumerable<UserContent>>(new GetUserContentQuery());
            IEnumerable<FeaturedContent> featured = await mediator.Query<GetFeaturedContentQuery, IEnumerable<FeaturedContent>>(new GetFeaturedContentQuery());

            IEnumerable<UserContentToBeFeaturedViewModel> vms = mapper.Map<IEnumerable<UserContent>, IEnumerable<UserContentToBeFeaturedViewModel>>(finalList);

            IEnumerable<UserContentToBeFeaturedViewModel> featurable = vms.Where(x => !string.IsNullOrWhiteSpace(x.Title));

            foreach (UserContentToBeFeaturedViewModel item in featurable)
            {
                FeaturedContent featuredNow = featured.FirstOrDefault(x => x.UserContentId == item.Id && x.StartDate.ToLocalTime() <= DateTime.Now.ToLocalTime() && (!x.EndDate.HasValue || (x.EndDate.HasValue && x.EndDate.Value.ToLocalTime() > DateTime.Now.ToLocalTime())));

                SetProperties(item, featuredNow);

                CalculateScore(item);
            }

            featurable = featurable.Where(x => x.HasFeaturedImage).ToList();

            FeaturedContentScreenViewModel resultVm = new FeaturedContentScreenViewModel
            {
                Featured = featurable.Where(x => x.IsFeatured).OrderByDescending(x => x.CreateDate).ToList(),
                NotFeatured = featurable.Where(x => !x.IsFeatured).OrderByDescending(x => x.Score).ThenByDescending(x => x.CreateDate).ToList(),
            };

            return new OperationResultVo<FeaturedContentScreenViewModel>(resultVm);
        }

        public async Task<OperationResultVo> Unfeature(Guid userId, Guid id)
        {
            try
            {
                FeaturedContent existing = await mediator.Query<GetFeaturedContentByIdQuery, FeaturedContent>(new GetFeaturedContentByIdQuery(id));

                if (existing != null)
                {
                    existing.EndDate = DateTime.Now;

                    existing.Active = false;

                    CommandResult result = await mediator.SendCommand(new SaveFeaturedContentCommand(userId, existing));

                    if (!result.Validation.IsValid)
                    {
                        string message = result.Validation.Errors.FirstOrDefault().ErrorMessage;
                        return new OperationResultVo<Guid>(existing.Id, false, message);
                    }
                    else
                    {
                        return new OperationResultVo<Guid>(existing.Id, 0);
                    }
                }

                return new OperationResultVo(true);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        private static void SetProperties(UserContentToBeFeaturedViewModel item, FeaturedContent featuredNow)
        {
            if (featuredNow != null)
            {
                item.CurrentFeatureId = featuredNow.Id;
            }

            item.IsFeatured = item.CurrentFeatureId.HasValue;

            item.AuthorName = string.IsNullOrWhiteSpace(item.AuthorName) ? Constants.UnknownSoul : item.AuthorName;

            item.TitleCompliant = !string.IsNullOrWhiteSpace(item.Title) && item.Title.Length <= 30;

            item.TitleLength = string.IsNullOrWhiteSpace(item.Title) ? 0 : item.Title.Length;

            item.IntroCompliant = !string.IsNullOrWhiteSpace(item.Introduction) && item.Introduction.Length <= 120;

            item.IntroLength = string.IsNullOrWhiteSpace(item.Introduction) ? 0 : item.Introduction.Length;

            item.ContentCompliant = !string.IsNullOrWhiteSpace(item.Content) && item.Content.Length >= 250;

            item.ContentLength = string.IsNullOrWhiteSpace(item.Content) ? 0 : item.Content.Length;

            item.IsArticle = !string.IsNullOrWhiteSpace(item.Title) && !string.IsNullOrWhiteSpace(item.Introduction);

            item.HasFeaturedImage = !string.IsNullOrWhiteSpace(item.FeaturedImage) && !item.FeaturedImage.Equals(Constants.DefaultFeaturedImage);
        }

        private static void CalculateScore(UserContentToBeFeaturedViewModel item)
        {
            if (item.TitleCompliant)
            {
                item.Score += 2;
            }

            if (item.IntroCompliant)
            {
                item.Score += 2;
            }

            if (item.ContentCompliant)
            {
                item.Score += 1;
            }
        }
    }
}