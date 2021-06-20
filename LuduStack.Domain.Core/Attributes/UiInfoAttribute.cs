using System;

namespace LuduStack.Domain.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class UiInfoAttribute : Attribute
    {
        public int Order { get; set; }
        public int Type { get; set; }
        public int SubType { get; set; }
        public string Class { get; set; }
        public string Locale { get; set; }
        public string Description { get; set; }
        public string Display { get; set; }
        public string DefaultValue { get; set; }
    }
}