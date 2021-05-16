using LuduStack.Domain.Core.Enums;
using System;

namespace LuduStack.Domain.ValueObjects
{
    public class MediaListItemVo
    {
        public DateTime CreateDate { get; set; }

        public SupportedLanguage? Language { get; set; }

        public MediaType? Type { get; set; }

        public string Url { get; set; }

        public string UrlLquip { get; set; }

        public string UrlResponsive { get; set; }

        public MediaListItemVo()
        {
            CreateDate = DateTime.Now;
        }
    }
}