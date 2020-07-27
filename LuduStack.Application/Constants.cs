namespace LuduStack.Application
{
    public static class Constants
    {
        public static string UnknownSoul
        {
            get
            {
                return "Unknown soul";
            }
        }

        public static string SoundOfSilence
        {
            get
            {
                return "this is the sound... of silence...";
            }
        }

        public static string DefaultProfileDescription
        {
            get
            {
                return "is a game developer willing to rock the game development world with funny games.";
            }
        }

        public static string DefaultUsername
        {
            get
            {
                return "theuser";
            }
        }

        public static string DefaultAvatar
        {
            get
            {
                return "/images/profileimages/developer.png";
            }
        }

        public static string DefaultProfileCoverImage
        {
            get
            {
                return "/images/placeholders/profilecoverimage.jpg";
            }
        }

        public static string DefaultGameCoverImage
        {
            get
            {
                return "/images/placeholders/gamecoverimage.jpg";
            }
        }

        public static string DefaultGameThumbnail
        {
            get
            {
                return "/images/placeholders/gameplaceholder.png";
            }
        }

        public static string DefaultFeaturedImage
        {
            get
            {
                return "/images/placeholders/featuredimage.jpg";
            }
        }

        public static string DefaultFeaturedImageLquip
        {
            get
            {
                return "/images/placeholders/featuredimagelquip.jpg";
            }
        }

        public static string DefaultImagePath
        {
            get
            {
                return "/storage/image";
            }
        }

        public static string DefaultUserImagePath
        {
            get
            {
                return "/storage/userimage";
            }
        }

        public static string DefaultCloudinaryPath
        {
            get
            {
                return "https://res.cloudinary.com/ludustack/image/upload/f_auto/";
            }
        }

        public static string DefaultAvatarPlaceholder
        {
            get
            {
                return string.Format("{0},q_auto/v1/xpto/profileimage_xpto_Personal", DefaultCloudinaryPath);
            }
        }

        public static string DefaultImagePlaceholder
        {
            get
            {
                return string.Format("{0},q_auto/v1/xpto", DefaultCloudinaryPath);
            }
        }

        public static string DefaultLuduStackPath
        {
            get
            {
                return "https://www.ludustack.com/";
            }
        }

        public static string DefaultCdnPath
        {
            get
            {
                return "https://ludustackcdn.azureedge.net/";
            }
        }

        public static string DefaultAzureStoragePath
        {
            get
            {
                return "https://ludustack.blob.core.windows.net/";
            }
        }

        public static string DefaultCourseThumbnail
        {
            get
            {
                return "/images/placeholders/courseplaceholder.png";
            }
        }

        public static string DefaultGiveawayThumbnail
        {
            get
            {
                return "/images/placeholders/giveawayplaceholder.png";
            }
        }

        public static string TimeZones
        {
            get
            {
                return @"[{""Value"":""-12"",""Text"":""(UTC -12) Eniwetok, Kwajalein""},{""Value"":""-11"",""Text"":""(UTC -11) Midway Island, Samoa""},{""Value"":""-10"",""Text"":""(UTC -10) Hawaii""},{""Value"":""-9"",""Text"":""(UTC -9) Alaska""},{""Value"":""-8"",""Text"":""(UTC -8) Pacific Time (US & Canada)""},{""Value"":""-7"",""Text"":""(UTC -7) Mountain Time (US & Canada)""},{""Value"":""-6"",""Text"":""(UTC -6) Central Time (US & Canada), Mexico City""},{""Value"":""-5"",""Text"":""(UTC -5) Eastern Time (US & Canada), Bogota, Lima""},{""Value"":""-4"",""Text"":""(UTC -4) Atlantic Time (Canada), Caracas, La Paz""},{""Value"":""-3.5"",""Text"":""(UTC -3:30) Newfoundland""},{""Value"":""-3"",""Text"":""(UTC -3) Brazil, Buenos Aires, Georgetown""},{""Value"":""-2"",""Text"":""(UTC -2) Mid-Atlantic""},{""Value"":""-1"",""Text"":""(UTC -1) Azores, Cape Verde Islands""},{""Value"":""0"",""Text"":""(UTC) Western Europe Time, London, Lisbon, Casablanca""},{""Value"":""1"",""Text"":""(UTC +1) Brussels, Copenhagen, Madrid, Paris""},{""Value"":""2"",""Text"":""(UTC +2) Kaliningrad, South Africa""},{""Value"":""3"",""Text"":""(UTC +3) Baghdad, Riyadh, Moscow, St. Petersburg""},{""Value"":""3.5"",""Text"":""(UTC +3:30) Tehran""},{""Value"":""4"",""Text"":""(UTC +4) Abu Dhabi, Muscat, Baku, Tbilisi""},{""Value"":""4.5"",""Text"":""(UTC +4:30) Kabul""},{""Value"":""5"",""Text"":""(UTC +5) Ekaterinburg, Islamabad, Karachi, Tashkent""},{""Value"":""5.5"",""Text"":""(UTC +5:30) Bombay, Calcutta, Madras, New Delhi""},{""Value"":""5.75"",""Text"":""(UTC +5:45) Kathmandu""},{""Value"":""6"",""Text"":""(UTC +6) Almaty, Dhaka, Colombo""},{""Value"":""7"",""Text"":""(UTC +7) Bangkok, Hanoi, Jakarta""},{""Value"":""8"",""Text"":""(UTC +8) Beijing, Perth, Singapore, Hong Kong""},{""Value"":""9"",""Text"":""(UTC +9) Tokyo, Seoul, Osaka, Sapporo, Yakutsk""},{""Value"":""9.5"",""Text"":""(UTC +9:30) Adelaide, Darwin""},{""Value"":""10"",""Text"":""(UTC +10) Eastern Australia, Guam, Vladivostok""},{""Value"":""11"",""Text"":""(UTC +11) Magadan, Solomon Islands, New Caledonia""},{""Value"":""12"",""Text"":""(UTC +12) Auckland, Wellington, Fiji, Kamchatka""}]";
            }
        }
    }
}