using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Models;
using System;

namespace LuduStack.Domain.Models
{
    public class LocalizationEntry : Entity
    {
        public Guid TermId { get; set; }

        public LocalizationLanguage Language { get; set; }

        public string Value { get; set; }

        public bool? Accepted { get; set; }
    }
}