using System;
using System.IO;
using System.Linq;
using System.Text;

namespace LuduStack.Domain.Core.Extensions
{
    public static class StringExtensions
    {
        public static string FirstCharToUpper(this string input)
        {
            switch (input)
            {
                case null: throw new ArgumentNullException(nameof(input));
                case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
                default: return input.First().ToString().ToUpper() + input.Substring(1);
            }
        }

        public static string GetFirstWords(this string input, int count)
        {
            return string.Join(' ', input.GetWords(count, StringSplitOptions.RemoveEmptyEntries));
        }

        public static string[] GetWords(this string input)
        {
            return GetWords(input, -1, null, StringSplitOptions.None);
        }

        public static string[] GetWords(this string input, int count)
        {
            return GetWords(input, count, null, StringSplitOptions.None);
        }

        public static string[] GetWords(this string input, StringSplitOptions options)
        {
            return GetWords(input, -1, null, options);
        }

        public static string[] GetWords(this string input, int count, StringSplitOptions options)
        {
            return GetWords(input, count, null, options);
        }

        public static string[] GetWords(this string input, int count, string[] wordDelimiter, StringSplitOptions options)
        {
            if (string.IsNullOrEmpty(input))
            {
                return new string[] { };
            }

            if (count < 0)
            {
                return input.Split(wordDelimiter, options);
            }

            string[] words = input.Split(wordDelimiter, count + 1, options);
            if (words.Length <= count)
            {
                return words;
            }

            Array.Resize(ref words, words.Length - 1);

            return words;
        }

        public static string NoExtension(this string input)
        {
            return Path.GetFileNameWithoutExtension(input);
        }

        public static string ReplaceCloudname(this string originalString)
        {
            if (string.IsNullOrWhiteSpace(originalString))
            {
                return originalString;
            }

            return originalString.Replace("https://res.cloudinary.com/indievisible", "https://res.cloudinary.com/ludustack");
        }

        /// <summary>
        /// Produces optional, URL-friendly version of a title, "like-this-one".
        /// hand-tuned for speed, reflects performance refactoring contributed
        /// by John Gietzen (user otac0n on Stack Overflow)
        /// </summary>
        public static string Slugify(this string text)
        {
            if (text == null)
            {
                return "";
            }

            const int maxlen = 80;
            int len = text.Length;
            bool prevdash = false;
            StringBuilder sb = new StringBuilder(len);
            char c;

            for (int i = 0; i < len; i++)
            {
                c = text[i];
                if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
                {
                    sb.Append(c);
                    prevdash = false;
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    // tricky way to convert to lowercase
                    sb.Append((char)(c | 32));
                    prevdash = false;
                }
                else if (c == ' ' || c == ',' || c == '.' || c == '/' ||
                    c == '\\' || c == '-' || c == '_' || c == '=')
                {
                    if (!prevdash && sb.Length > 0)
                    {
                        sb.Append('-');
                        prevdash = true;
                    }
                }
                else if (c >= 128)
                {
                    int prevlen = sb.Length;
                    sb.Append(RemapInternationalCharToAscii(c));
                    if (prevlen != sb.Length)
                    {
                        prevdash = false;
                    }
                }
                if (i == maxlen)
                {
                    break;
                }
            }

            if (prevdash)
            {
                return sb.ToString().Substring(0, sb.Length - 1);
            }
            else
            {
                return sb.ToString();
            }
        }

        private static string RemapInternationalCharToAscii(char c)
        {
            string s = c.ToString().ToLowerInvariant();
            if ("àåáâäãåą".Contains(s))
            {
                return "a";
            }
            else if ("èéêëę".Contains(s))
            {
                return "e";
            }
            else if ("ìíîïı".Contains(s))
            {
                return "i";
            }
            else if ("òóôõöøőð".Contains(s))
            {
                return "o";
            }
            else if ("ùúûüŭů".Contains(s))
            {
                return "u";
            }
            else if ("çćčĉ".Contains(s))
            {
                return "c";
            }
            else if ("żźž".Contains(s))
            {
                return "z";
            }
            else if ("śşšŝ".Contains(s))
            {
                return "s";
            }
            else if ("ñń".Contains(s))
            {
                return "n";
            }
            else if ("ýÿ".Contains(s))
            {
                return "y";
            }
            else if ("ğĝ".Contains(s))
            {
                return "g";
            }
            else if (c == 'ř')
            {
                return "r";
            }
            else if (c == 'ł')
            {
                return "l";
            }
            else if (c == 'đ')
            {
                return "d";
            }
            else if (c == 'ß')
            {
                return "ss";
            }
            else if (c == 'Þ')
            {
                return "th";
            }
            else if (c == 'ĥ')
            {
                return "h";
            }
            else if (c == 'ĵ')
            {
                return "j";
            }
            else
            {
                return "";
            }
        }
    }
}