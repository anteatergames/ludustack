﻿using CloudinaryDotNet;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Text;
using LuduStack.Domain.Core;
using Microsoft.AspNetCore.Http;
using System.Runtime.CompilerServices;
using LuduStack.Application.Helpers;

namespace LuduStack.Application.Formatters
{
    public static class UrlFormatter
    {
        public static string FormatFeaturedImageUrl(Guid userId, string featuredImage, ImageRenderType type)
        {
            if (!string.IsNullOrWhiteSpace(featuredImage) && !featuredImage.Contains("/images/placeholders/"))
            {
                switch (type)
                {
                    case ImageRenderType.LowQuality:
                        return Image(userId, ImageType.FeaturedImage, featuredImage, 600, 10);

                    case ImageRenderType.Responsive:
                        return Image(userId, ImageType.FeaturedImage, featuredImage, true, 0, 0);

                    case ImageRenderType.Full:
                    default:
                        return Image(userId, ImageType.FeaturedImage, featuredImage);
                }
            }
            else
            {
                return featuredImage;
            }
        }

        public static string FormatFeaturedVideoUrl(Guid userId, string featuredVideo)
        {
            return Video(userId, ImageType.FeaturedImage, featuredVideo);
        }

        #region Internal

        public static string GetDefaultImage(ImageType type)
        {
            string defaultImageNotRooted = string.Empty;

            switch (type)
            {
                case ImageType.ProfileImage:
                    defaultImageNotRooted = Constants.DefaultAvatar;
                    break;

                case ImageType.ProfileCover:
                    defaultImageNotRooted = Constants.DefaultProfileCoverImage;
                    break;

                case ImageType.GameThumbnail:
                    defaultImageNotRooted = Constants.DefaultGameThumbnail;
                    break;

                case ImageType.GameCover:
                    defaultImageNotRooted = Constants.DefaultGameCoverImage;
                    break;

                case ImageType.ContentImage:
                    defaultImageNotRooted = Constants.DefaultGameThumbnail;
                    break;

                case ImageType.FeaturedImage:
                    defaultImageNotRooted = Constants.DefaultFeaturedImage;
                    break;

                default:
                    defaultImageNotRooted = Constants.DefaultAvatar;
                    break;
            }

            defaultImageNotRooted = defaultImageNotRooted.Substring(1).Replace(@"/", @"\");

            return defaultImageNotRooted;
        }

        public static string ProfileImage(Guid userId)
        {
            return ProfileImage(userId, null, 0);
        }

        public static string ProfileImage(Guid userId, int width)
        {
            return ProfileImage(userId, null, width);
        }

        public static string ProfileImage(Guid userId, DateTime? lastUpdateDate)
        {
            return ProfileImage(userId, lastUpdateDate, 0);
        }

        public static string ProfileImage(Guid userId, DateTime? lastUpdateDate, int width)
        {
            string fileName = string.Format("profileimage_{0}_Personal", userId);

            string url = CdnImageCommon(userId, fileName, false, width, 0, Constants.DefaultAvatar);

            return url.ReplaceCloudname();
        }

        public static string ProfileCoverImage(Guid userId, Guid profileId)
        {
            return ProfileCoverImage(userId, profileId, null, false, 0);
        }

        public static string ProfileCoverImage(Guid userId, Guid profileId, DateTime? lastUpdateDate, bool hasCoverImage)
        {
            return ProfileCoverImage(userId, profileId, lastUpdateDate, hasCoverImage, 0);
        }

        public static string ProfileCoverImage(Guid userId, Guid profileId, DateTime? lastUpdateDate, bool hasCoverImage, int width)
        {
            long v = lastUpdateDate.HasValue ? lastUpdateDate.Value.Ticks : 1;

            string fileName = string.Format("profilecover_{0}", profileId);

            string url = hasCoverImage ? string.Format("{0}/{1}/{2}?v={3}", Constants.DefaultCdnPath, userId, fileName, v) : Constants.DefaultProfileCoverImage;

            url = url.Replace("//", "/").Replace("https:/", "https://");

            if (hasCoverImage && !url.Equals(Constants.DefaultGameCoverImage))
            {
                url = CdnImageCommon(userId, fileName, width);
            }

            return url.ReplaceCloudname();
        }

        public static string Image(Guid userId, ImageType type, string fileName)
        {
            return Image(userId, type, fileName, 0);
        }

        public static string Image(Guid userId, ImageType type, string fileName, int width)
        {
            return Image(userId, type, fileName, width, 0);
        }

        public static string Image(Guid userId, ImageType type, string fileName, int width, int quality)
        {
            return Image(userId, type, fileName, false, width, quality, Constants.DefaultFeaturedImage);
        }

        public static string Image(Guid userId, ImageType type, string fileName, bool responsive, int width, int quality)
        {
            return Image(userId, type, fileName, responsive, width, quality, Constants.DefaultFeaturedImage);
        }

        public static string Image(Guid userId, ImageType type, string fileName, bool responsive, int width, int quality, string defaultImage)
        {
            if (Constants.DefaultGameThumbnail.Contains(fileName) || Constants.DefaultGiveawayThumbnail.Contains(fileName))
            {
                return fileName;
            }

            string url = CdnImageCommon(userId, fileName, responsive, width, quality, defaultImage);

            return url.ReplaceCloudname();
        }

        public static string Video(Guid userId, ImageType type, string featuredVideo)
        {
            if (Constants.DefaultGameThumbnail.Contains(featuredVideo) || Constants.DefaultGiveawayThumbnail.Contains(featuredVideo))
            {
                return featuredVideo;
            }

            string url = CdnVideoCommon(userId, featuredVideo);

            return url.ReplaceCloudname();
        }

        public static string CdnImageCommon(Guid userId, string fileName)
        {
            return CdnImageCommon(userId, fileName, false, 0, 0, Constants.DefaultFeaturedImage);
        }

        public static string CdnImageCommon(Guid userId, string fileName, int width)
        {
            return CdnImageCommon(userId, fileName, false, width, 0, Constants.DefaultFeaturedImage);
        }

        public static string CdnImageCommon(Guid userId, string fileName, bool responsive, int width, int quality, string defaultImage)
        {
            try
            {
                string publicId = GetPublicId(userId, ref fileName);

                Cloudinary cloudinary = new Cloudinary(ConfigHelper.ConfigOptions.CloudinaryUrl);

                if (responsive)
                {
                    StringBuilder sb = new StringBuilder();

                    Transformation transformation = new Transformation().FetchFormat("auto").Width(300);
                    string url300 = cloudinary.Api.UrlImgUp.Secure(true).Transform(transformation).BuildUrl(publicId);
                    sb.Append(string.Format("{0} 300w, ", url300));

                    transformation = new Transformation().FetchFormat("auto").Width(600);
                    string url600 = cloudinary.Api.UrlImgUp.Secure(true).Transform(transformation).BuildUrl(publicId);
                    sb.Append(string.Format("{0} 600w, ", url600));

                    transformation = new Transformation().FetchFormat("auto");
                    string url900 = cloudinary.Api.UrlImgUp.Secure(true).Transform(transformation).BuildUrl(publicId);

                    sb.Append(string.Format("{0} 900w", url900));

                    return sb.ToString();
                }
                else
                {
                    Transformation transformation = new Transformation().FetchFormat("auto");

                    if (width > 0)
                    {
                        transformation = transformation.Width(width);
                    }

                    if (quality > 0)
                    {
                        transformation = transformation.Quality(quality).Crop("crop");
                    }
                    else
                    {
                        transformation = transformation.Quality("auto");
                    }

                    string finalUrl = cloudinary.Api.UrlImgUp.Secure(true).Transform(transformation).BuildUrl(publicId);

                    return finalUrl;
                }
            }
            catch (Exception)
            {
                return defaultImage;
            }
        }

        private static string CdnVideoCommon(Guid userId, string featuredVideo)
        {
            string publicId = GetPublicId(userId, ref featuredVideo);

            Cloudinary cloudinary = new Cloudinary(ConfigHelper.ConfigOptions.CloudinaryUrl);

            Transformation transformation = new Transformation().FetchFormat("auto");

            string finalUrl = cloudinary.Api.UrlVideoUp.Secure(true).Transform(transformation).BuildUrl(publicId);

            return finalUrl;
        }

        private static string GetPublicId(Guid userId, ref string fileName)
        {
            string[] fileNameSplit = fileName.Split('/');
            if (fileNameSplit.Length > 1)
            {
                fileName = fileNameSplit.Last();
            }

            string publicId = string.Format("{0}/{1}", userId, fileName.Split('.')[0]);
            return publicId;
        }

        #endregion Internal

        #region ExternalUrls

        private static string ExternalUrlCommon(string handler)
        {
            if (!string.IsNullOrWhiteSpace(handler))
            {
                handler = handler.ToLower().Replace(" ", "-");
            }

            return handler;
        }

        private static string CompleteUrlCommon(string handler)
        {
            if (!string.IsNullOrWhiteSpace(handler))
            {
                handler = handler.Trim('/');
                if (!handler.StartsWith("http"))
                {
                    handler = string.Format("http://{0}", handler);
                }
            }

            return handler;
        }

        #region Profiles

        public static string ItchIoProfile(string handler)
        {
            if (!string.IsNullOrWhiteSpace(handler) && !handler.Contains("itch.io"))
            {
                handler = ExternalUrlCommon(handler);
                return string.Format("https://{0}.itch.io", handler);
            }
            else
            {
                handler = CompleteUrlCommon(handler);
                return handler;
            }
        }

        public static string GameJoltProfile(string handler)
        {
            if (!string.IsNullOrWhiteSpace(handler) && !handler.Contains("gamejolt.com"))
            {
                handler = ExternalUrlCommon(handler);
                return string.Format("https://gamejolt.com/@{0}", handler);
            }
            else
            {
                handler = CompleteUrlCommon(handler);
                return handler;
            }
        }

        public static string UnityConnectProfile(string handler)
        {
            if (!string.IsNullOrWhiteSpace(handler) && !handler.Contains("connect.unity.com"))
            {
                handler = ExternalUrlCommon(handler);
                return string.Format("https://connect.unity.com/u/{0}", handler);
            }
            else
            {
                handler = CompleteUrlCommon(handler);
                return handler;
            }
        }

        public static string IndieDbPofile(string handler)
        {
            if (!string.IsNullOrWhiteSpace(handler) && !handler.Contains("indiedb.com"))
            {
                handler = ExternalUrlCommon(handler);
                return string.Format("https://www.indiedb.com/members/{0}", handler);
            }
            else
            {
                handler = CompleteUrlCommon(handler);
                return handler;
            }
        }

        public static string GamedevNetProfile(string handler)
        {
            if (!string.IsNullOrWhiteSpace(handler) && !handler.Contains("gamedev.net"))
            {
                handler = ExternalUrlCommon(handler);
                return string.Format("https://www.gamedev.net/profile/{0}", handler);
            }
            else
            {
                handler = CompleteUrlCommon(handler);
                return handler;
            }
        }

        public static string ReplaceCloudVersion(string thumbnailUrl)
        {
            return thumbnailUrl.Replace("/v1/", string.Format("/v{0}/", DateTime.Now.Ticks));
        }

        public static string AppleAppStoreProfile(string handler)
        {
            if (!string.IsNullOrWhiteSpace(handler) && !handler.Contains("apps.apple.com"))
            {
                handler = ExternalUrlCommon(handler);
                return string.Format("https://apps.apple.com/us/developer/{0}", handler);
            }
            else
            {
                handler = CompleteUrlCommon(handler);
                return handler;
            }
        }

        public static string GooglePlayStoreProfile(string handler)
        {
            if (!string.IsNullOrWhiteSpace(handler) && !handler.Contains("play.google.com"))
            {
                handler = ExternalUrlCommon(handler);
                return string.Format("https://play.google.com/store/apps/dev?id={0}", handler);
            }
            else
            {
                handler = CompleteUrlCommon(handler);
                return handler;
            }
        }

        public static string IndiExpoProfile(string handler)
        {
            if (!string.IsNullOrWhiteSpace(handler) && !handler.Contains("indiexpo.net"))
            {
                handler = ExternalUrlCommon(handler);
                return string.Format("https://www.indiexpo.net/users/{0}", handler);
            }
            else
            {
                handler = CompleteUrlCommon(handler);
                return handler;
            }
        }

        public static string ArtstationProfile(string handler)
        {
            if (!string.IsNullOrWhiteSpace(handler) && !handler.Contains("artstation.com"))
            {
                return string.Format("https://www.artstation.com/{0}", handler);
            }
            else
            {
                handler = CompleteUrlCommon(handler);
                return handler;
            }
        }

        public static string DeviantArtProfile(string handler)
        {
            if (!string.IsNullOrWhiteSpace(handler) && !handler.Contains("deviantart.com"))
            {
                return string.Format("https://www.deviantart.com/{0}", handler);
            }
            else
            {
                handler = CompleteUrlCommon(handler);
                return handler;
            }
        }

        public static string DevToProfile(string handler)
        {
            if (!string.IsNullOrWhiteSpace(handler) && !handler.Contains("dev.to"))
            {
                return string.Format("https://dev.to/{0}", handler);
            }
            else
            {
                handler = CompleteUrlCommon(handler);
                return handler;
            }
        }

        public static string GitHubProfile(string handler)
        {
            if (!string.IsNullOrWhiteSpace(handler) && !handler.Contains("github.com"))
            {
                return string.Format("https://github.com/{0}", handler);
            }
            else
            {
                handler = CompleteUrlCommon(handler);
                return handler;
            }
        }

        public static string HackerRankProfile(string handler)
        {
            if (!string.IsNullOrWhiteSpace(handler) && !handler.Contains("hackerrank.com"))
            {
                return string.Format("https://www.hackerrank.com/{0}", handler);
            }
            else
            {
                handler = CompleteUrlCommon(handler);
                return handler;
            }
        }

        public static string LinkedInProfile(string handler)
        {
            if (!string.IsNullOrWhiteSpace(handler) && !handler.Contains("linkedin.com"))
            {
                return string.Format("https://www.linkedin.com/in/{0}", handler);
            }
            else
            {
                handler = CompleteUrlCommon(handler);
                return handler;
            }
        }

        public static string PatreonProfile(string handler)
        {
            if (!string.IsNullOrWhiteSpace(handler) && !handler.Contains("patreon.com"))
            {
                return string.Format("https://www.patreon.com/{0}", handler);
            }
            else
            {
                handler = CompleteUrlCommon(handler);
                return handler;
            }
        }

        public static string MediumProfile(string handler)
        {
            if (!string.IsNullOrWhiteSpace(handler) && !handler.Contains("medium.com"))
            {
                return string.Format("https://medium.com/@{0}", handler);
            }
            else
            {
                handler = CompleteUrlCommon(handler);
                return handler;
            }
        }

        #endregion Profiles

        public static string Website(string handler)
        {
            handler = CompleteUrlCommon(handler);
            return handler;
        }

        public static string Facebook(string handler)
        {
            if (!string.IsNullOrWhiteSpace(handler) && !handler.Contains("facebook.com"))
            {
                handler = ExternalUrlCommon(handler);
                return string.Format("https://www.facebook.com/{0}", handler);
            }
            else
            {
                handler = CompleteUrlCommon(handler);
                return handler;
            }
        }

        public static string Twitter(string handler)
        {
            if (!string.IsNullOrWhiteSpace(handler) && !handler.Contains("twitter.com"))
            {
                handler = ExternalUrlCommon(handler);
                return string.Format("https://twitter.com/{0}", handler);
            }
            else
            {
                handler = CompleteUrlCommon(handler);
                return handler;
            }
        }

        public static string Instagram(string handler)
        {
            if (!string.IsNullOrWhiteSpace(handler) && !handler.Contains("instagram.com"))
            {
                handler = ExternalUrlCommon(handler);
                return string.Format("https://www.instagram.com/{0}", handler);
            }
            else
            {
                handler = CompleteUrlCommon(handler);
                return handler;
            }
        }

        public static string Youtube(string handler)
        {
            if (!string.IsNullOrWhiteSpace(handler) && !handler.Contains("youtube.com"))
            {
                handler = ExternalUrlCommon(handler);
                return string.Format("https://www.youtube.com/channel/{0}", handler);
            }
            else
            {
                handler = CompleteUrlCommon(handler);
                return handler;
            }
        }

        public static string XboxLiveProfile(string handler)
        {
            if (!string.IsNullOrWhiteSpace(handler) && !handler.Contains("xbox.com"))
            {
                handler = ExternalUrlCommon(handler);
                return string.Format("https://account.xbox.com/en-us/profile?gamertag={0}", handler);
            }
            else
            {
                handler = CompleteUrlCommon(handler);
                return handler;
            }
        }

        public static string PlayStationStoreProfile(string handler)
        {
            if (!string.IsNullOrWhiteSpace(handler) && !handler.Contains("playstation.com"))
            {
                handler = ExternalUrlCommon(handler);
                return string.Format("https://my.playstation.com/profile/{0}", handler);
            }
            else
            {
                handler = CompleteUrlCommon(handler);
                return handler;
            }
        }

        public static string SteamGame(string handler)
        {
            if (!string.IsNullOrWhiteSpace(handler) && !handler.Contains("store.steampowered.com"))
            {
                handler = ExternalUrlCommon(handler);
                return string.Format("https://store.steampowered.com/app/{0}", handler);
            }
            else
            {
                handler = CompleteUrlCommon(handler);
                return handler;
            }
        }

        public static string GameJoltGame(string handler)
        {
            if (!string.IsNullOrWhiteSpace(handler) && !handler.Contains("gamejolt.com"))
            {
                handler = ExternalUrlCommon(handler);
                return string.Format("https://gamejolt.com/games/{0}", handler);
            }
            else
            {
                handler = CompleteUrlCommon(handler);
                return handler;
            }
        }

        public static string ItchIoGame(string userBase, string handler)
        {
            if (!string.IsNullOrWhiteSpace(handler))
            {
                if (!handler.ToLower().Contains("itch.io"))
                {
                    userBase = userBase.Replace("https://", string.Empty).Replace("http://", string.Empty).TrimEnd('/');
                    handler = string.Format("https://{0}/{1}", userBase, handler.Trim('/'));
                }
                else if (handler.ToLower().Contains("itch.io") && !handler.ToLower().Contains("http"))
                {
                    handler = string.Format("https://{0}", handler.Trim('/'));
                }
            }

            return handler;
        }

        public static string GamedevNetGame(string handler)
        {
            return handler;
        }

        public static string IndieDbGame(string handler)
        {
            if (!string.IsNullOrWhiteSpace(handler) && !handler.Contains("indiedb.com"))
            {
                handler = ExternalUrlCommon(handler);
                return string.Format("https://www.indiedb.com/games/{0}", handler);
            }
            else
            {
                handler = CompleteUrlCommon(handler);
                return handler;
            }
        }

        public static string UnityConnectGame(string handler)
        {
            if (!string.IsNullOrWhiteSpace(handler) && !handler.Contains("connect.unity.com"))
            {
                handler = ExternalUrlCommon(handler);
                return string.Format("https://connect.unity.com/p/{0}", handler);
            }
            else
            {
                handler = CompleteUrlCommon(handler);
                return handler;
            }
        }

        public static string AppleAppStoreGame(string handler)
        {
            if (!string.IsNullOrWhiteSpace(handler) && !handler.Contains("apps.apple.com"))
            {
                handler = ExternalUrlCommon(handler);
                return string.Format("https://apps.apple.com/us/app/{0}", handler);
            }
            else
            {
                handler = CompleteUrlCommon(handler);
                return handler;
            }
        }

        public static string GooglePlayStoreGame(string handler)
        {
            if (!string.IsNullOrWhiteSpace(handler) && !handler.Contains("play.google.com"))
            {
                return string.Format("https://play.google.com/store/apps/details?id={0}", handler);
            }
            else
            {
                handler = CompleteUrlCommon(handler);
                return handler;
            }
        }

        public static string XboxLiveGame(string handler)
        {
            if (!string.IsNullOrWhiteSpace(handler) && !handler.Contains("xbox.com"))
            {
                handler = ExternalUrlCommon(handler);
                return string.Format("https://www.xbox.com/games/{0}", handler);
            }
            else
            {
                handler = CompleteUrlCommon(handler);
                return handler;
            }
        }

        public static string PlayStationStoreGame(string handler)
        {
            if (!string.IsNullOrWhiteSpace(handler) && !handler.Contains("store.playstation.com"))
            {
                return string.Format("https://store.playstation.com/en-us/product/{0}", handler);
            }
            else
            {
                handler = CompleteUrlCommon(handler);
                return handler;
            }
        }

        public static string IndiExpoGame(string handler)
        {
            if (!string.IsNullOrWhiteSpace(handler) && !handler.Contains("indiexpo.net"))
            {
                return string.Format("https://www.indiexpo.net/games/{0}", handler);
            }
            else
            {
                handler = CompleteUrlCommon(handler);
                return handler;
            }
        }

        public static string DiscordGame(string handler)
        {
            if (!string.IsNullOrWhiteSpace(handler) && !handler.Contains("discord.gg"))
            {
                return string.Format("https://discord.gg/{0}", handler);
            }
            else
            {
                handler = CompleteUrlCommon(handler);
                return handler;
            }
        }

        internal static string DiscordProfile(string handler)
        {
            if (!string.IsNullOrWhiteSpace(handler) && !handler.Contains("discord.gg"))
            {
                return string.Format("https://discord.gg/{0}", handler);
            }
            else
            {
                handler = CompleteUrlCommon(handler);
                return handler;
            }
        }

        #endregion ExternalUrls
    }
}