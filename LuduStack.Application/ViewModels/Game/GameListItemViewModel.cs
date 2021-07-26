using LuduStack.Domain.Core.Attributes;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Extensions;
using LuduStack.Domain.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuduStack.Application.ViewModels.Game
{
    public class GameListItemViewModel : GameBaseViewModel, IGameCardBasic
    {
        public string Price { get; set; }

        public string Platforms { get; set; }

        private List<string> _platformList;

        public List<string> PlatformList
        {
            get
            {
                if (_platformList == null || !_platformList.Any())
                {
                    _platformList = PopulatePlatforms();
                }

                return _platformList;
            }
        }

        private List<string> PopulatePlatforms()
        {
            List<string> platformList = new List<string>();

            if (!string.IsNullOrWhiteSpace(Platforms))
            {
                string[] values = Platforms.Split('|');

                values.Where(x => !string.IsNullOrWhiteSpace(x)).ToList().ForEach(x =>
                {
                    bool convertionOK = Enum.TryParse<GamePlatforms>(x, out GamePlatforms parsedValue);

                    if (convertionOK)
                    {
                        string uiClass = parsedValue.GetAttributeOfType<UiInfoAttribute>().Class;

                        platformList.Add(uiClass);
                    }
                });
            }

            return platformList;
        }
    }
}