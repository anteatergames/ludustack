using LuduStack.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuduStack.Domain.ValueObjects
{
    public class ImageListItemVo
    {
        public SupportedLanguage Language { get; set; }

        public string Image { get; set; }
        public string ImageLquip { get; set; }
        public string ImageResponsive { get; set; }
    }
}
