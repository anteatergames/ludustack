using LuduStack.Application.Interfaces;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Messaging;
using System;

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