using LuduStack.Domain.Core.Enums;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace LuduStack.Web.Helpers
{
    public static class ContentFormatter
    {
        public static string FormatCFormatTextAreaBreaks(string content)
        {
            return content.Replace("<br>", string.Empty);
        }

        public static string FormatContentToShow(string content)
        {
            //group 1 figure class="xxxx">
            //group 2 oembed start
            //group 3 <img src="
            //group 5
            //group 6 abre parenteses
            //group 7 <a href=" indica url já formatada
            //group 8 url
            //group 9 path example: /path/to/folder/before/resource
            //group 10 fecha parenteses
            //group 11 >
            //group 12 figure ending
            //group 13 </a>
            //group 17 </oembed>
            //group 18 </figure>

            string patternUrl = @"(<figure class="".+?"">)?(<oembed url=""|<div data-oembed-url=""|<oembed>)?(<img(.?)?(data-)?src="")?(\()?(\<a href\=\"")?([(http(s)?:\/\/)?(www\.)?a-zA-Z0-9\-@:%._\+~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#?&//=\,\;]*)(\/[\w|\?|\=|\&|\;|\-\%\.]+)?)(\/)?""?(>)?([(http(s)?:\/\/)?(www\.)?a-zA-Z0-9\-@:%._\+~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#?&//=\,\;]*)(\/[\w|\?|\=|\&|\;|\-\%\.]+)?)?(\/)?( )?(<\/a>)?(\))?(<\/oembed>|><\/oembed>)?(<\/figure>)?(><\/figure>)?(.>)?";

            Regex theRegex = new Regex(patternUrl);

            MatchCollection matchesUrl = theRegex.Matches(content);

            foreach (Match match in matchesUrl)
            {
                string toReplace = match.Groups[0].Value;
                string oembedPrefix = match.Groups[2].Value;
                string imagePrefix = match.Groups[3].Value;
                string openParenthesis = match.Groups[6].Value;
                string url = match.Groups[8].Value;
                string closeParenthesis = match.Groups[10].Value;

                url = !url.TrimStart('(').TrimEnd(')').ToLower().StartsWith("http") ? String.Format("http://{0}", url) : url;

                string newText = string.Empty;
                if (!string.IsNullOrWhiteSpace(imagePrefix))
                {
                    newText = String.Format("<img src=\"{0}\" />", url);
                }
                else if (!string.IsNullOrWhiteSpace(oembedPrefix))
                {
                    newText = String.Format(@"<div class=""videoWrapper""><oembed>{0}</oembed></div>", url);
                }
                else
                {
                    newText = string.Format(@"<a href=""{0}"" target=""_blank"" style=""font-weight:500"" rel=""noopener"">{0}</a>", url);
                }

                if (!string.IsNullOrWhiteSpace(openParenthesis) && !string.IsNullOrWhiteSpace(closeParenthesis))
                {
                    newText = String.Format("({0})", newText);
                }

                string templateUrlCkEditor = String.Format("<a href=\"{0}\">.+</a>", url);

                bool isAlreadyUrl = Regex.IsMatch(content, templateUrlCkEditor);

                if (!isAlreadyUrl)
                {
                    content = content.Replace(toReplace, newText);
                }
            }

            content = FormatHashTagsToShow(content);

            return content;
        }

        public static string FormatHashTagsToShow(string content)
        {
            string patternHashtag = @"(\#\w+)";

            Regex regexHashtag = new Regex(patternHashtag);

            MatchCollection matchesHashtag = regexHashtag.Matches(content);

            foreach (Match match in matchesHashtag)
            {
                string toReplace = match.Groups[0].Value.Trim();

                string formattedLink = String.Format(@"<a href=""/search/?q={0}"" class=""hashtag"">{1}</a>", HttpUtility.UrlEncode(toReplace), toReplace);

                content = content.Replace(toReplace, formattedLink);
            }

            return content;
        }

        internal static string FormatUrlContentToShow(UserContentType type)
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

        public static string GetYoutubeVideoId(string url)
        {
            var uri = new Uri(url);
            var query = HttpUtility.ParseQueryString(uri.Query);

            var videoId = string.Empty;

            if (query.AllKeys.Contains("v"))
            {
                videoId = query["v"];
            }
            else
            {
                videoId = uri.Segments.Last();
            }

            return videoId;
        }
    }
}