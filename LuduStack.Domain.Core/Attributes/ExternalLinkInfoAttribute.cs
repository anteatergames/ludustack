using LuduStack.Domain.Core.Enums;
using System;

namespace LuduStack.Domain.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ExternalLinkInfoAttribute : UiInfoAttribute
    {
        public new ExternalLinkType Type { get; set; }
        public bool IsStore { get; set; }
        public string ColorClass { get; set; }
    }
}