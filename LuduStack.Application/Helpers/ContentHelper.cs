using LuduStack.Application.Formatters;
using LuduStack.Domain.Core.Enums;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace LuduStack.Application.Helpers
{
    public static class ContentHelper
    {
        public static string FormatFeaturedImageUrl(Guid userId, string featuredImage, ImageRenderType type)
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

        public static string FormatFeaturedVideoUrl(Guid userId, string featuredVideo)
        {
            return UrlFormatter.Video(userId, ImageType.FeaturedImage, featuredVideo);
        }

        public static MediaType GetMediaType(string featuredImage)
        {
            if (string.IsNullOrWhiteSpace(featuredImage))
            {
                return MediaType.None;
            }

            string youtubePattern = @"(https?\:\/\/)?(www\.youtube\.com|youtu\.?be)\/.+";

            Match match = Regex.Match(featuredImage, youtubePattern);

            if (match.Success)
            {
                return MediaType.Youtube;
            }

            string[] imageExtensions = new string[] { "jpg", "png", "gif", "tiff", "webp", "svg", "jfif", "jpeg", "bmp" };
            string[] videoExtensions = new string[] { "mp4", "avi", "mpeg", "vob", "webm", "mpg", "m4v", "wmv", "asf", "mov", "mpe", "3gp" };

            string extension = featuredImage.Split('.').Last();

            if (imageExtensions.Contains(extension.ToLower()))
            {
                return MediaType.Image;
            }
            else if (videoExtensions.Contains(extension.ToLower()))
            {
                return MediaType.Video;
            }

            return MediaType.Image;
        }
    }
}