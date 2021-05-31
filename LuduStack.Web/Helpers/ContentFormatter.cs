using LuduStack.Domain.Core.Enums;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace LuduStack.Web.Helpers
{
    public static class ContentFormatter
    {
        public static string FormatCFormatTextAreaBreaks(string content)
        {
            return content?.Replace("<br>", string.Empty);
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

                url = !url.TrimStart('(').TrimEnd(')').ToLower().StartsWith("http") ? string.Format("http://{0}", url) : url;

                string newText = string.Empty;
                if (!string.IsNullOrWhiteSpace(imagePrefix))
                {
                    newText = string.Format("<img src=\"{0}\" />", url);
                }
                else if (!string.IsNullOrWhiteSpace(oembedPrefix))
                {
                    newText = string.Format(@"<div class=""videowrapper""><oembed>{0}</oembed></div>", url);
                }
                else
                {
                    newText = string.Format(@"<a href=""{0}"" target=""_blank"" style=""font-weight:500"" rel=""noopener"">{0}</a>", url);
                }

                if (!string.IsNullOrWhiteSpace(openParenthesis) && !string.IsNullOrWhiteSpace(closeParenthesis))
                {
                    newText = string.Format("({0})", newText);
                }

                string templateUrlCkEditor = string.Format("<a href=\"{0}\">.+</a>", url);

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

                string formattedLink = string.Format(@"<a href=""/search/?q={0}"" class=""hashtag"">{1}</a>", HttpUtility.UrlEncode(toReplace), toReplace);

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
            Uri uri = new Uri(url);
            NameValueCollection query = HttpUtility.ParseQueryString(uri.Query);

            if (query.AllKeys.Contains("v"))
            {
                return query["v"];
            }
            else
            {
                return uri.Segments.Last();
            }
        }

        public static string DateTimeToCreatedAgoMessage(DateTime createDate, IStringLocalizer localizer)
        {
            const int SECOND = 1;
            const int MINUTE = 60 * SECOND;
            const int HOUR = 60 * MINUTE;
            const int DAY = 24 * HOUR;
            const int MONTH = 30 * DAY;

            TimeSpan ts = new TimeSpan(DateTime.UtcNow.Ticks - createDate.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 1 * MINUTE)
            {
                return ts.Seconds == 1 ? localizer["one second ago"] : string.Format("{0} {1}", ts.Seconds, localizer["seconds ago"]);
            }

            if (delta < 2 * MINUTE)
            {
                return localizer["a minute ago"];
            }

            if (delta < 45 * MINUTE)
            {
                return string.Format("{0} {1}", ts.Minutes, localizer["minutes ago"]);
            }

            if (delta < 90 * MINUTE)
            {
                return localizer["an hour ago"];
            }

            if (delta < 24 * HOUR)
            {
                return string.Format("{0} {1}", ts.Hours, localizer["hours ago"]);
            }

            if (delta < 48 * HOUR)
            {
                return localizer["yesterday"];
            }

            if (delta < 30 * DAY)
            {
                return string.Format("{0} {1}", ts.Days, localizer["days ago"]);
            }

            if (delta < 12 * MONTH)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? localizer["one month ago"] : string.Format("{0} {1}", months, localizer["months ago"]);
            }
            else
            {
                int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                return years <= 1 ? localizer["one year ago"] : string.Format("{0} {1}", years, localizer["years ago"]);
            }
        }
    }
}