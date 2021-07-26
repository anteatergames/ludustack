using System;
using System.Collections.Specialized;
using System.Linq;
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