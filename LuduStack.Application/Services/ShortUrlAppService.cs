using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.ShortUrl;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Messaging.Queries.ShortUrl;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Messaging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LuduStack.Application.Services
{
    public class ShortUrlAppService : BaseAppService, IShortUrlAppService
    {
        private readonly IShortUrlDomainService shortUrlDomainService;

        public ShortUrlAppService(IMediatorHandler mediator, IBaseAppServiceCommon baseAppServiceCommon, IShortUrlDomainService shortUrlDomainService) : base(baseAppServiceCommon)
        {
            this.mediator = mediator;
            this.shortUrlDomainService = shortUrlDomainService;
        }

        #region ICrudAppService

        public async Task<OperationResultVo<int>> Count(Guid currentUserId)
        {
            try
            {
                int count = await mediator.Query<CountShortUrlQuery, int>(new CountShortUrlQuery());

                return new OperationResultVo<int>(count);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<int>(ex.Message);
            }
        }

        public async Task<OperationResultListVo<ShortUrlViewModel>> GetAll(Guid currentUserId)
        {
            try
            {
                IEnumerable<ShortUrl> allModels = shortUrlDomainService.GetAll();

                IEnumerable<ShortUrlViewModel> vms = mapper.Map<IEnumerable<ShortUrl>, IEnumerable<ShortUrlViewModel>>(allModels);

                return new OperationResultListVo<ShortUrlViewModel>(vms);
            }
            catch (Exception ex)
            {
                return new OperationResultListVo<ShortUrlViewModel>(ex.Message);
            }
        }

        public async Task<OperationResultVo> GetAllIds(Guid currentUserId)
        {
            try
            {
                IEnumerable<Guid> allIds = shortUrlDomainService.GetAllIds();

                return new OperationResultListVo<Guid>(allIds);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public async Task<OperationResultVo<ShortUrlViewModel>> GetById(Guid currentUserId, Guid id)
        {
            try
            {
                ShortUrl model = await mediator.Query<GetShortUrlByIdQuery, ShortUrl>(new GetShortUrlByIdQuery(id));

                ShortUrlViewModel vm = mapper.Map<ShortUrlViewModel>(model);

                return new OperationResultVo<ShortUrlViewModel>(vm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<ShortUrlViewModel>(ex.Message);
            }
        }

        public async Task<OperationResultVo<Guid>> Save(Guid currentUserId, ShortUrlViewModel viewModel)
        {
            try
            {
                ShortUrl model;

                ShortUrl existing = await mediator.Query<GetShortUrlByIdQuery, ShortUrl>(new GetShortUrlByIdQuery(viewModel.Id));

                if (existing != null)
                {
                    model = mapper.Map(viewModel, existing);
                }
                else
                {
                    model = mapper.Map<ShortUrl>(viewModel);
                }

                if (viewModel.Id == Guid.Empty)
                {
                    shortUrlDomainService.Add(model);
                    viewModel.Id = model.Id;
                }
                else
                {
                    shortUrlDomainService.Update(model);
                }

                await unitOfWork.Commit();

                return new OperationResultVo<Guid>(model.Id);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<Guid>(ex.Message);
            }
        }

        public async Task<OperationResultVo> Remove(Guid currentUserId, Guid id)
        {
            try
            {
                // validate before

                shortUrlDomainService.Remove(id);

                await unitOfWork.Commit();

                return new OperationResultVo(true);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        #endregion ICrudAppService

        public OperationResultVo GetFullUrlByToken(string token)
        {
            try
            {
                ShortUrl shortUrl = shortUrlDomainService.GetByToken(token);

                if (shortUrl != null)
                {
                    shortUrl.AccessCount = shortUrl.AccessCount + 1;

                    shortUrlDomainService.Update(shortUrl);

                    unitOfWork.Commit();
                }

                return new OperationResultVo<string>(shortUrl?.OriginalUrl, 0);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }
    }
}