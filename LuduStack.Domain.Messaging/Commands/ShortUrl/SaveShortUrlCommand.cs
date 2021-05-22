using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Models;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging
{
    public class SaveShortUrlCommand : BaseCommand
    {
        public ShortUrl ShortUrl { get; }

        public SaveShortUrlCommand(ShortUrl shortUrl) : base(shortUrl.Id)
        {
            ShortUrl = shortUrl;
        }

        public SaveShortUrlCommand(string urlReferal, ShortUrlDestinationType type) : base(Guid.Empty)
        {
            ShortUrl newShortUrl = new ShortUrl
            {
                OriginalUrl = urlReferal,
                DestinationType = type
            };

            ShortUrl = newShortUrl;
        }

        public override bool IsValid()
        {
            Result.Validation = new SaveShortUrlCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class SaveShortUrlCommandHandler : CommandHandler, IRequestHandler<SaveShortUrlCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IShortUrlRepository shortUrlRepository;

        private static readonly Random random = new Random();

        private const string BASEURL = "/go/";

        private const string ALPHABET = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public SaveShortUrlCommandHandler(IUnitOfWork unitOfWork, IShortUrlRepository shortUrlRepository)
        {
            this.unitOfWork = unitOfWork;
            this.shortUrlRepository = shortUrlRepository;
        }

        public async Task<CommandResult> Handle(SaveShortUrlCommand request, CancellationToken cancellationToken)
        {
            CommandResult result = request.Result;

            if (!request.IsValid()) { return request.Result; }

            if (request.ShortUrl.Id == Guid.Empty)
            {
                string newToken = Encode(5);

                while (shortUrlRepository.CountDirectly(x => x.Token.Equals(newToken)) > 1)
                {
                    newToken = Encode(5);
                }

                request.ShortUrl.Token = newToken;
                request.ShortUrl.NewUrl = string.Format("{0}{1}", BASEURL, newToken);

                await shortUrlRepository.Add(request.ShortUrl);
            }
            else
            {
                shortUrlRepository.Update(request.ShortUrl);
            }

            result.Validation = await Commit(unitOfWork);

            return result;
        }

        private string Encode(int length)
        {
            string newToken = new string(Enumerable.Repeat(ALPHABET, length).Select(s => s[random.Next(s.Length)]).ToArray());

            return newToken;
        }
    }
}