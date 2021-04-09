using AutoMapper.QueryableExtensions;
using LuduStack.Application.Formatters;
using LuduStack.Application.Helpers;
using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Content;
using LuduStack.Application.ViewModels.FeaturedContent;
using LuduStack.Application.ViewModels.Home;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces.Services;
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
        private readonly IFeaturedContentDomainService featuredContentDomainService;

        public FeaturedContentAppService(IBaseAppServiceCommon baseAppServiceCommon
            , IFeaturedContentDomainService featuredContentDomainService) : base(baseAppServiceCommon)
        {
            this.featuredContentDomainService = featuredContentDomainService;
        }

        public CarouselViewModel GetFeaturedNow()
        {
            IQueryable<FeaturedContent> allModels = featuredContentDomainService.GetFeaturedNow();

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

                    vm.FeaturedImage = ContentHelper.SetFeaturedImage(userId, imageSplit.Last(), ImageRenderType.Full);
                    vm.FeaturedImageLquip = ContentHelper.SetFeaturedImage(userId, imageSplit.Last(), ImageRenderType.LowQuality);
                }

                return model;
            }
            else
            {
                CarouselViewModel fake = FakeData.FakeCarousel();

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

        public async Task<IEnumerable<UserContentToBeFeaturedViewModel>> GetContentToBeFeatured()
        {
            IEnumerable<UserContent> finalList = await mediator.Query<GetUserContentQuery, IEnumerable<UserContent>>(new GetUserContentQuery());
            IEnumerable<FeaturedContent> featured = await mediator.Query<GetFeaturedContentQuery, IEnumerable<FeaturedContent>>(new GetFeaturedContentQuery());

            IEnumerable<UserContentToBeFeaturedViewModel> vms = mapper.Map<IEnumerable<UserContent>, IEnumerable<UserContentToBeFeaturedViewModel>>(finalList);

            foreach (UserContentToBeFeaturedViewModel item in vms)
            {
                FeaturedContent featuredNow = featured.FirstOrDefault(x => x.UserContentId == item.Id && x.StartDate.ToLocalTime() <= DateTime.Now.ToLocalTime() && (!x.EndDate.HasValue || (x.EndDate.HasValue && x.EndDate.Value.ToLocalTime() > DateTime.Now.ToLocalTime())));

                if (featuredNow != null)
                {
                    item.CurrentFeatureId = featuredNow.Id;
                }

                item.IsFeatured = item.CurrentFeatureId.HasValue;

                item.AuthorName = string.IsNullOrWhiteSpace(item.AuthorName) ? Constants.UnknownSoul : item.AuthorName;

                item.TitleCompliant = !string.IsNullOrWhiteSpace(item.Title) && item.Title.Length <= 25;

                item.IntroCompliant = !string.IsNullOrWhiteSpace(item.Introduction) && item.Introduction.Length <= 55;

                item.ContentCompliant = !string.IsNullOrWhiteSpace(item.Content) && item.Content.Length >= 800;

                item.IsArticle = !string.IsNullOrWhiteSpace(item.Title) && !string.IsNullOrWhiteSpace(item.Introduction);
            }

            vms = vms.OrderByDescending(x => x.IsFeatured).ToList();

            return vms;
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
    }
}