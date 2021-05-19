using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging
{
    public class AddImagesToGameCommand : BaseCommand
    {
        public List<MediaListItemVo> Media { get; }

        public AddImagesToGameCommand(Guid gameId, List<MediaListItemVo> media) : base(gameId)
        {
            Media = media;
        }

        public override bool IsValid()
        {
            Result.Validation = new AddImagesToGameCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class AddImagesToGameCommandHandler : CommandHandler, IRequestHandler<AddImagesToGameCommand, CommandResult>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IGameRepository gameRepository;

        public AddImagesToGameCommandHandler(IUnitOfWork unitOfWork, IGameRepository gameRepository)
        {
            this.unitOfWork = unitOfWork;
            this.gameRepository = gameRepository;
        }

        public async Task<CommandResult> Handle(AddImagesToGameCommand request, CancellationToken cancellationToken)
        {
            CommandResult result = request.Result;

            if (!request.IsValid()) { return request.Result; }

            Game game = await gameRepository.GetById(request.Id);

            if (game != null)
            {
                bool gameUpdated = false;

                if (game.Media == null)
                {
                    game.Media = new List<MediaListItemVo>();
                }

                foreach (MediaListItemVo mediaItem in request.Media)
                {
                    bool alreadyExists = game.Media.FirstOrDefault(x => x.Url.Equals(mediaItem.Url)) != null;

                    if (!alreadyExists)
                    {
                        game.Media.Add(mediaItem);
                        gameUpdated = true;
                    }
                }

                if (gameUpdated)
                {
                    gameRepository.Update(game);
                }

                result.Validation = await Commit(unitOfWork);
            }

            return result;
        }
    }
}