using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Web;

namespace LuduStack.Application.Formatters
{
    public static class ContentFormatter
    {
        public static string FormatCFormatTextAreaBreaks(string content)
        {
            return content?.Replace("<br>", string.Empty);
        }

        public static string FormatContentToShow(string content)
        {

            content = FormatVideoContentToShow(content);

            content = FormatHashTagsToShow(content);

            return content;
        }

        public static string FormatVideoContentToShow(string content)
        {
            //group 0 the content to be replaced
            //group 2 <div data-oembed-url="
            //group 3 url
            //group 4 url second half
            //group 7 </figure

            string patternUrl = @"<figure class="".+?"">(\s+)?(<div data-oembed-url="")+([(http(s)?:\/\/)?(www\.)?a-zA-Z0-9\-@:%._\+~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#?&//=\,\;]*)(\/[\w|\?|\=|\&|\;|\-\%\.]+)?)(\/)?""?>(\s+.+)?(.+)?[<\figure>]";

            Regex videoRegex = new Regex(patternUrl);

            MatchCollection matchesUrl = videoRegex.Matches(content);

            foreach (Match match in matchesUrl)
            {
                string toReplace = match.Groups[0].Value;
                string url = match.Groups[3].Value;

                url = !url.TrimStart('(').TrimEnd(')').ToLower().StartsWith("http") ? string.Format("http://{0}", url) : url;

                string newText = string.Format(@"<div class=""videowrapper""><oembed>{0}</oembed></div>", url);

                content = content.Replace(toReplace, newText);
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

        public static StringDictionary SaveReplacements()
        {
            return new StringDictionary
            {
                { "<figure><table>", @"<table>" },
                { "<figure class=\"table\"><table>", @"<table>" },
                { "</table></figure>", @"</table>" },
            };
        }

        public static StringDictionary DisplayReplacements()
        {
            return new StringDictionary
            {
                { "<img src=", @"<img class=""img-fluid m-0"" src=" },
                { "<div data-oembed-url=", @"<div class=""col-12 col-lg-8 col-xl-6 mx-auto"" data-oembed-url=" },
                { "<figure class=\"", @"<figure class=""image text-center mx-auto " },
                { "<figure", @"<div class=""text-center mx-auto""><figure class=""mx-auto""" },
                { "</figure>", @"</figure></div>" },
                { "<figcaption>", @"<figcaption class=""text-center"">" },
                { "<pre>", @"<pre class=""p-0 p-md-2 p-lg-3 p-xl-4"">" },
                { "<iframe src=", @"<iframe frameBorder=""0"" src=" },
                { "<table>", @"<div class=""table-responsive""><table class=""table table-hover"">" },
                { "</table>", @"</table></div>" },
            };
        }
    }
}