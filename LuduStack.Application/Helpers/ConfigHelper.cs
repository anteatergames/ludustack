using LuduStack.Application.ViewModels.PlatformSetting;
using LuduStack.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuduStack.Application.Helpers
{
    public static class ConfigHelper
    {
        private static ConfigOptions configOptions;

        public static ConfigOptions ConfigOptions { get { return configOptions ?? new ConfigOptions();  } private set { configOptions = value; } }

        public static void SetConfigOptions(ConfigOptions configOptions)
        {
            ConfigOptions = configOptions;
        }
    }
}
