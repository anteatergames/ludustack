using LuduStack.Application.Formatters;
using LuduStack.Domain.Core.Enums;
using System;

namespace LuduStack.Application.Helpers
{
    public static class ContentHelper
    {
        public static string SetFeaturedImage(Guid userId, string featuredImage, ImageRenderType type)
        {
            if (!string.IsNullOrWhiteSpace(featuredImage) && !featuredImage.Contains("/images/placeholders/"))
            {
                switch (type)
                {
                    case ImageRenderType.LowQuality:
                        return UrlFormatter.Image(userId, ImageType.FeaturedImage, featuredImage, 600, 10);

                    case ImageRenderType.Responsive:
                        return UrlFormatter.Image(userId, ImageType.FeaturedImage, featuredImage, true, 0, 0);

                    case ImageRenderType.Full:
                    default:
                        return UrlFormatter.Image(userId, ImageType.FeaturedImage, featuredImage);
                }
            }
            else
            {
                return featuredImage;
            }
        }
    }
}