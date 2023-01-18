namespace LuduStack.Application.Helpers
{
    public static class ConfigHelper
    {
        private static ConfigOptions configOptions;

        public static ConfigOptions ConfigOptions { get { return configOptions ?? new ConfigOptions(); } private set { configOptions = value; } }

        public static void SetConfigOptions(ConfigOptions configOptions)
        {
            ConfigOptions = configOptions;
        }
    }
}
