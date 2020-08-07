using AutoMapper;
using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.ShortUrl;
using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Infrastructure;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using System;
using System.Collections.Generic;

namespace LuduStack.Application.Services
{
    public class ShortUrlAppService : BaseAppService, IShortUrlAppService
    {
        private readonly IShortUrlDomainService shortUrlDomainService;

        public ShortUrlAppService(IBaseAppServiceCommon baseAppServiceCommon, IShortUrlDomainService shortUrlDomainService) : base(baseAppServiceCommon)
        {
            this.shortUrlDomainService = shortUrlDomainService;
        }

        #region ICrudAppService

        public OperationResultVo<int> Count(Guid currentUserId)
        {
            try
            {
                int count = shortUrlDomainService.Count();

                return new OperationResultVo<int>(count);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<int>(ex.Message);
            }
        }

        public OperationResultListVo<ShortUrlViewModel> GetAll(Guid currentUserId)
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

        public OperationResultVo GetAllIds(Guid currentUserId)
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

        public OperationResultVo<ShortUrlViewModel> GetById(Guid currentUserId, Guid id)
        {
            try
            {
                ShortUrl model = shortUrlDomainService.GetById(id);

                ShortUrlViewModel vm = mapper.Map<ShortUrlViewModel>(model);

                return new OperationResultVo<ShortUrlViewModel>(vm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<ShortUrlViewModel>(ex.Message);
            }
        }

        public OperationResultVo<Guid> Save(Guid currentUserId, ShortUrlViewModel viewModel)
        {
            try
            {
                ShortUrl model;

                ShortUrl existing = shortUrlDomainService.GetById(viewModel.Id);

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

                unitOfWork.Commit();

                return new OperationResultVo<Guid>(model.Id);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<Guid>(ex.Message);
            }
        }

        public OperationResultVo Remove(Guid currentUserId, Guid id)
        {
            try
            {
                // validate before

                shortUrlDomainService.Remove(id);

                unitOfWork.Commit();

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