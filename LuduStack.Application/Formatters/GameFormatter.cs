using LuduStack.Application.ViewModels;
using LuduStack.Application.ViewModels.Game;
using LuduStack.Domain.Core.Attributes;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Extensions;
using LuduStack.Domain.Messaging.Queries.UserProfile;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Application.Formatters
{
    public static class GameFormatter
    {
        public static void FilCharacteristics(GameViewModel vm)
        {
            if (vm.Characteristics == null)
            {
                vm.Characteristics = new List<GameCharacteristicVo>();
            }
            List<GameCharacteristcs> allBenefits = Enum.GetValues(typeof(GameCharacteristcs)).Cast<GameCharacteristcs>().Where(x => x != GameCharacteristcs.NotInformed).ToList();

            foreach (GameCharacteristcs characteristic in allBenefits)
            {
                if (!vm.Characteristics.Any(x => x.Characteristic == characteristic))
                {
                    vm.Characteristics.Add(new GameCharacteristicVo { Characteristic = characteristic, Available = false });
                }
            }
        }

        public static async Task FormatExternalLinks(IMediatorHandler mediator, GameViewModel vm)
        {
            IEnumerable<UserProfile> profiles = await mediator.Query<GetUserProfileByUserIdQuery, IEnumerable<UserProfile>>(new GetUserProfileByUserIdQuery(vm.UserId));
            UserProfile authorProfile = profiles.FirstOrDefault();
            ExternalLinkVo itchProfile = authorProfile.ExternalLinks.FirstOrDefault(x => x.Provider == ExternalLinkProvider.ItchIo);

            foreach (ExternalLinkBaseViewModel item in vm.ExternalLinks)
            {
                ExternalLinkInfoAttribute uiInfo = item.Provider.GetAttributeOfType<ExternalLinkInfoAttribute>();
                item.Display = uiInfo.Display;
                item.IconClass = uiInfo.Class;
                item.ColorClass = uiInfo.ColorClass;
                item.IsStore = uiInfo.IsStore;
                item.Order = uiInfo.Order;

                item.Value = CheckProvider(itchProfile, item);
            }
        }

        private static string CheckProvider(ExternalLinkVo itchProfile, ExternalLinkBaseViewModel item)
        {
            switch (item.Provider)
            {
                case ExternalLinkProvider.Website:
                    return UrlFormatter.Website(item.Value);

                case ExternalLinkProvider.Facebook:
                    return UrlFormatter.Facebook(item.Value);

                case ExternalLinkProvider.Twitter:
                    return UrlFormatter.Twitter(item.Value);

                case ExternalLinkProvider.Instagram:
                    return UrlFormatter.Instagram(item.Value);

                case ExternalLinkProvider.Youtube:
                    return UrlFormatter.Youtube(item.Value);

                case ExternalLinkProvider.XboxLive:
                    return UrlFormatter.XboxLiveGame(item.Value);

                case ExternalLinkProvider.PlaystationStore:
                    return UrlFormatter.PlayStationStoreGame(item.Value);

                case ExternalLinkProvider.Steam:
                    return UrlFormatter.SteamGame(item.Value);

                case ExternalLinkProvider.GameJolt:
                    return UrlFormatter.GameJoltGame(item.Value);

                case ExternalLinkProvider.ItchIo:
                    return UrlFormatter.ItchIoGame(itchProfile?.Value, item.Value);

                case ExternalLinkProvider.GamedevNet:
                    return UrlFormatter.GamedevNetGame(item.Value);

                case ExternalLinkProvider.IndieDb:
                    return UrlFormatter.IndieDbGame(item.Value);

                case ExternalLinkProvider.UnityConnect:
                    return UrlFormatter.UnityConnectGame(item.Value);

                case ExternalLinkProvider.GooglePlayStore:
                    return UrlFormatter.GooglePlayStoreGame(item.Value);

                case ExternalLinkProvider.AppleAppStore:
                    return UrlFormatter.AppleAppStoreGame(item.Value);

                case ExternalLinkProvider.IndiExpo:
                    return UrlFormatter.IndiExpoGame(item.Value);

                case ExternalLinkProvider.Discord:
                    return UrlFormatter.DiscordGame(item.Value);

                default:
                    return UrlFormatter.Website(item.Value);
            }
        }
    }
}