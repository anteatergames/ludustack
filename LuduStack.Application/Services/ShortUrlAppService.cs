using LuduStack.Application.Interfaces;
using LuduStack.Domain.Messaging;
using LuduStack.Domain.Messaging.Queries.ShortUrl;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Messaging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Application.Services
{
    public class ShortUrlAppService : BaseAppService, IShortUrlAppService
    {
        public ShortUrlAppService(IMediatorHandler mediator, IBaseAppServiceCommon baseAppServiceCommon) : base(baseAppServiceCommon)
        {
            this.mediator = mediator;
        }

        public async Task<OperationResultVo> GetFullUrlByToken(string token)
        {
            try
            {
                ShortUrl shortUrl = await mediator.Query<GetShortUrlByTokenQuery, ShortUrl>(new GetShortUrlByTokenQuery(token));

                if (shortUrl != null)
                {
                    shortUrl.AccessCount = shortUrl.AccessCount + 1;

                    CommandResult result = await mediator.SendCommand(new SaveShortUrlCommand(shortUrl));

                    if (!result.Validation.IsValid)
                    {
                        string message = result.Validation.Errors.FirstOrDefault().ErrorMessage;
                        return new OperationResultVo<string>(shortUrl.OriginalUrl, false, message);
                    }
                    else
                    {
                        return new OperationResultVo<string>(shortUrl.OriginalUrl, 0);
                    }
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