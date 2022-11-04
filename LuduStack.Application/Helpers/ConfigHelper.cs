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
        public static ConfigOptions ConfigOptions { get; private set; }

        public static void SetConfigOptions(ConfigOptions configOptions)
        {
            ConfigOptions = configOptions;
        }
    }
}
