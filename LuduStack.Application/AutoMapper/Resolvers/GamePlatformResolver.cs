using AutoMapper;
using LuduStack.Application.ViewModels.Game;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuduStack.Application.AutoMapper.Resolvers
{
    public class GamePlatformToDomainResolver : IValueResolver<GameViewModel, Game, string>
    {
        public string Resolve(GameViewModel source, Game destination, string destMember, ResolutionContext context)
        {
            string result = string.Empty;

            if (source.Platforms == null || !source.Platforms.Any())
            {
                return result;
            }

            result = string.Join('|', source.Platforms);

            return result;
        }
    }

    public class GamePlatformFromDomainResolver : IValueResolver<Game, GameViewModel, List<GamePlatforms>>
    {
        public List<GamePlatforms> Resolve(Game source, GameViewModel destination, List<GamePlatforms> destMember, ResolutionContext context)
        {
            string[] platforms = (source.Platforms ?? string.Empty)
                .Replace("XboxOne", "Xbox")
                .Replace("Playstation4", "Playstation")
                .Split(new[] { '|' });

            IEnumerable<GamePlatforms> platformsConverted = platforms.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => (GamePlatforms)Enum.Parse(typeof(GamePlatforms), x));

            return platformsConverted.ToList();
        }
    }
}