using Ganss.Xss;
using LuduStack.Domain.Core.Enums;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace LuduStack.Application.Helpers
{
    public static class ContentHelper
    {
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

            string[] imageExtensions = new[] { "jpg", "png", "gif", "tiff", "webp", "svg", "jfif", "jpeg", "bmp" };
            string[] videoExtensions = new[] { "mp4", "avi", "mpeg", "vob", "webm", "mpg", "m4v", "wmv", "asf", "mov", "mpe", "3gp" };

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

        public static string GetSpecialPostTemplate(UserContentType type)
        {
            switch (type)
            {
                case UserContentType.TeamCreation:
                    return "<div class=\"row p-3 \"><div class=\"col-12 col-md-4 p-2 text-center align-middle\"><i class=\"fas fa-4x fa-users\"></i></div><div class=\"col-12 col-md-8\">{0}. <br> <br> <span class=\"font-weight-bold text-uppercase\">{1}</span> <br> {2}</div></div>";

                case UserContentType.JobPosition:
                    return "<div class=\"row p-3 \"><div class=\"col-12 col-md-4 p-2 text-center align-middle\"><i class=\"fas fa-4x fa-briefcase\"></i></div><div class=\"col-12 col-md-8\">{0}. <br> <br> <span class=\"font-weight-bold text-uppercase\">{1}</span> <br> <span class=\"text-capitalize\">{2}</span></div></div>";

                default:
                    return "Check this out!";
            }
        }

        public static HtmlSanitizer GetHtmlSanitizer()
        {
            HtmlSanitizer sanitizer = new HtmlSanitizer();
            sanitizer.AllowedTags.Add("iframe");
            sanitizer.AllowedAttributes.Add("class");
            sanitizer.AllowedAttributes.Add("data-oembed-url");
            sanitizer.AllowedCssProperties.Remove("margin");
            sanitizer.AllowedCssProperties.Add("position");

            return sanitizer;
        }
    }
}