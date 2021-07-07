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

                switch (item.Provider)
                {
                    case ExternalLinkProvider.Website:
                        item.Value = UrlFormatter.Website(item.Value);
                        break;

                    case ExternalLinkProvider.Facebook:
                        item.Value = UrlFormatter.Facebook(item.Value);
                        break;

                    case ExternalLinkProvider.Twitter:
                        item.Value = UrlFormatter.Twitter(item.Value);
                        break;

                    case ExternalLinkProvider.Instagram:
                        item.Value = UrlFormatter.Instagram(item.Value);
                        break;

                    case ExternalLinkProvider.Youtube:
                        item.Value = UrlFormatter.Youtube(item.Value);
                        break;

                    case ExternalLinkProvider.XboxLive:
                        item.Value = UrlFormatter.XboxLiveGame(item.Value);
                        break;

                    case ExternalLinkProvider.PlaystationStore:
                        item.Value = UrlFormatter.PlayStationStoreGame(item.Value);
                        break;

                    case ExternalLinkProvider.Steam:
                        item.Value = UrlFormatter.SteamGame(item.Value);
                        break;

                    case ExternalLinkProvider.GameJolt:
                        item.Value = UrlFormatter.GameJoltGame(item.Value);
                        break;

                    case ExternalLinkProvider.ItchIo:
                        item.Value = UrlFormatter.ItchIoGame(itchProfile?.Value, item.Value);
                        break;

                    case ExternalLinkProvider.GamedevNet:
                        item.Value = UrlFormatter.GamedevNetGame(item.Value);
                        break;

                    case ExternalLinkProvider.IndieDb:
                        item.Value = UrlFormatter.IndieDbGame(item.Value);
                        break;

                    case ExternalLinkProvider.UnityConnect:
                        item.Value = UrlFormatter.UnityConnectGame(item.Value);
                        break;

                    case ExternalLinkProvider.GooglePlayStore:
                        item.Value = UrlFormatter.GooglePlayStoreGame(item.Value);
                        break;

                    case ExternalLinkProvider.AppleAppStore:
                        item.Value = UrlFormatter.AppleAppStoreGame(item.Value);
                        break;

                    case ExternalLinkProvider.IndiExpo:
                        item.Value = UrlFormatter.IndiExpoGame(item.Value);
                        break;

                    case ExternalLinkProvider.Discord:
                        item.Value = UrlFormatter.DiscordGame(item.Value);
                        break;
                }
            }
        }
    }
}