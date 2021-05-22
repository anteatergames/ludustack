﻿using System;
using System.IO;

namespace LuduStack.Domain.Core.Extensions
{
    public static class StringExtensions
    {
        public static string GetFirstWords(this string input, int count)
        {
            return string.Join(' ', input.GetWords(count, null, StringSplitOptions.RemoveEmptyEntries));
        }

        public static string[] GetWords(
            this string input,
            int count = -1,
            string[] wordDelimiter = null,
            StringSplitOptions options = StringSplitOptions.None)
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
    }
}