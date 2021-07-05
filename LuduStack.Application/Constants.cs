using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Text.Json;

namespace LuduStack.Application
{
    public static class Constants
    {
        public static string ProductionEnvironmentName => "env-production";

        public static string SiteName => "LUDUSTACK";

        public static string DefaultTitle => "LUDUSTACK - The one stop place for game developers";

        public static string DefaultDescription => "The one stop place for game developers, artists and musicians. Helping making gamedev dreams come true.";

        public static string DefaultOgType => "website";

        public static IEnumerable<SelectListItem> TimeZoneSelectList => JsonSerializer.Deserialize<IEnumerable<SelectListItem>>(Constants.TimeZones);

        public static string UnknownSoul => "Unknown soul";

        public static string SoundOfSilence => "this is the sound... of silence...";

        public static string DefaultProfileDescription => "is a game developer willing to rock the game development world with funny games.";

        public static string DefaultUsername => "theuser";

        public static string DefaultAvatar => "/images/placeholders/developer.png";

        public static string DefaultAvatar30 => "/images/placeholders/developer_w30.png";

        public static string DefaultProfileCoverImage => "/images/placeholders/profilecoverimage.jpg";

        public static string DefaultGameCoverImage => "/images/placeholders/gamecoverimage.jpg";

        public static string DefaultGameThumbnail => "/images/placeholders/gameplaceholder.png";

        public static string DefaultFeaturedImage => "/images/placeholders/featuredimage.jpg";

        public static string DefaultFeaturedImageLquip => "/images/placeholders/featuredimagelquip.jpg";

        public static string DefaultImagePath => "/storage/image";

        public static string DefaultUserImagePath => "/storage/userimage";

        public static string DefaultCloudinaryPath => "https://res.cloudinary.com/ludustack/image/upload/f_auto";

        public static string DefaultAvatarPlaceholder => string.Format("{0},q_auto/xpto/profileimage_xpto_Personal", DefaultCloudinaryPath);

        public static string DefaultImagePlaceholder => string.Format("{0},q_auto/xpto", DefaultCloudinaryPath);

        public static string DefaultLuduStackPath => "https://www.ludustack.com/";

        public static string DefaultCdnPath => "https://ludustackcdn.azureedge.net/";

        public static string DefaultAzureStoragePath => "https://ludustack.blob.core.windows.net/";

        public static string DefaultCourseThumbnail => "/images/placeholders/default1200x630.jpg";

        public static string DefaultGamejamThumbnail => "/images/placeholders/default1200x630.jpg";

        public static string DefaultGiveawayThumbnail => "/images/placeholders/giveawayplaceholder.png";

        public static string DefaultComicStripPlaceholder => "/images/placeholders/comicstripolaceholder.jpg";

        public static string TimeZones => @"[{""Value"":""-12"",""Text"":""(UTC -12) Eniwetok, Kwajalein""},{""Value"":""-11"",""Text"":""(UTC -11) Midway Island, Samoa""},{""Value"":""-10"",""Text"":""(UTC -10) Hawaii""},{""Value"":""-9"",""Text"":""(UTC -9) Alaska""},{""Value"":""-8"",""Text"":""(UTC -8) Pacific Time (US & Canada)""},{""Value"":""-7"",""Text"":""(UTC -7) Mountain Time (US & Canada)""},{""Value"":""-6"",""Text"":""(UTC -6) Central Time (US & Canada), Mexico City""},{""Value"":""-5"",""Text"":""(UTC -5) Eastern Time (US & Canada), Bogota, Lima""},{""Value"":""-4"",""Text"":""(UTC -4) Atlantic Time (Canada), Caracas, La Paz""},{""Value"":""-3.5"",""Text"":""(UTC -3:30) Newfoundland""},{""Value"":""-3"",""Text"":""(UTC -3) Brazil, Buenos Aires, Georgetown""},{""Value"":""-2"",""Text"":""(UTC -2) Mid-Atlantic""},{""Value"":""-1"",""Text"":""(UTC -1) Azores, Cape Verde Islands""},{""Value"":""0"",""Text"":""(UTC) Western Europe Time, London, Lisbon, Casablanca""},{""Value"":""1"",""Text"":""(UTC +1) Brussels, Copenhagen, Madrid, Paris""},{""Value"":""2"",""Text"":""(UTC +2) Kaliningrad, South Africa""},{""Value"":""3"",""Text"":""(UTC +3) Baghdad, Riyadh, Moscow, St. Petersburg""},{""Value"":""3.5"",""Text"":""(UTC +3:30) Tehran""},{""Value"":""4"",""Text"":""(UTC +4) Abu Dhabi, Muscat, Baku, Tbilisi""},{""Value"":""4.5"",""Text"":""(UTC +4:30) Kabul""},{""Value"":""5"",""Text"":""(UTC +5) Ekaterinburg, Islamabad, Karachi, Tashkent""},{""Value"":""5.5"",""Text"":""(UTC +5:30) Bombay, Calcutta, Madras, New Delhi""},{""Value"":""5.75"",""Text"":""(UTC +5:45) Kathmandu""},{""Value"":""6"",""Text"":""(UTC +6) Almaty, Dhaka, Colombo""},{""Value"":""7"",""Text"":""(UTC +7) Bangkok, Hanoi, Jakarta""},{""Value"":""8"",""Text"":""(UTC +8) Beijing, Perth, Singapore, Hong Kong""},{""Value"":""9"",""Text"":""(UTC +9) Tokyo, Seoul, Osaka, Sapporo, Yakutsk""},{""Value"":""9.5"",""Text"":""(UTC +9:30) Adelaide, Darwin""},{""Value"":""10"",""Text"":""(UTC +10) Eastern Australia, Guam, Vladivostok""},{""Value"":""11"",""Text"":""(UTC +11) Magadan, Solomon Islands, New Caledonia""},{""Value"":""12"",""Text"":""(UTC +12) Auckland, Wellington, Fiji, Kamchatka""}]";

        public static int PagingThreshold => 5;

        public static int BigAvatarSize => 50;

        public static int SmallAvatarSize => 40;

        public static int TinyAvatarSize => 30;
    }
}