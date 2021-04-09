using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Models;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
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

        public SaveShortUrlCommandHandler(IUnitOfWork unitOfWork, IShortUrlRepository shortUrlRepository)
        {
            this.unitOfWork = unitOfWork;
            this.shortUrlRepository = shortUrlRepository;
        }

        public async Task<CommandResult> Handle(SaveShortUrlCommand request, CancellationToken cancellationToken)
        {
            CommandResult result = request.Result;

            if (!request.IsValid()) return request.Result;

            if (request.ShortUrl.Id == Guid.Empty)
            {
                shortUrlRepository.Add(request.ShortUrl);
            }
            else
            {
                shortUrlRepository.Update(request.ShortUrl);
            }

            result.Validation = await Commit(unitOfWork);

            return result;
        }
    }
}