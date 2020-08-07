using LuduStack.Application.Interfaces;
using LuduStack.Application.Services;
using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Infrastructure;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Services;
using LuduStack.Infra.CrossCutting.Abstractions;
using LuduStack.Infra.CrossCutting.Notifications;
using LuduStack.Infra.Data.Cache;
using LuduStack.Infra.Data.MongoDb.Context;
using LuduStack.Infra.Data.MongoDb.Interfaces;
using LuduStack.Infra.Data.MongoDb.Repository;
using LuduStack.Infra.Data.MongoDb.UoW;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace LuduStack.Infra.CrossCutting.IoC
{
    public static class NativeInjectorBootStrapper
    {
        public static void RegisterServices(IServiceCollection services)
        {
            // ASP.NET HttpContext dependency
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<ICacheService, CacheService>();

            #region Game

            services.AddScoped<IGameAppService, GameAppService>();
            services.AddScoped<IGameDomainService, GameDomainService>();
            services.AddScoped<IGameRepository, GameRepository>();

            #endregion Game

            #region Profile

            services.AddScoped<IProfileAppService, ProfileAppService>();
            services.AddScoped<IProfileDomainService, ProfileDomainService>();
            services.AddScoped<IUserProfileRepository, UserProfileRepository>();

            #endregion Profile

            #region Content

            services.AddScoped<IUserContentAppService, UserContentAppService>();
            services.AddScoped<IUserContentDomainService, UserContentDomainService>();
            services.AddScoped<IUserContentRepository, UserContentRepository>();

            #endregion Content

            #region Brainstorm

            services.AddScoped<IBrainstormAppService, BrainstormAppService>();
            services.AddScoped<IBrainstormDomainService, BrainstormDomainService>();
            services.AddScoped<IBrainstormRepository, BrainstormRepository>();

            #endregion Brainstorm

            #region Featuring

            services.AddScoped<IFeaturedContentAppService, FeaturedContentAppService>();
            services.AddScoped<IFeaturedContentDomainService, FeaturedContentDomainService>();
            services.AddScoped<IFeaturedContentRepository, FeaturedContentRepository>();

            #endregion Featuring

            #region Preferences

            services.AddScoped<IUserPreferencesAppService, UserPreferencesAppService>();
            services.AddScoped<IUserPreferencesDomainService, UserPreferencesDomainService>();
            services.AddScoped<IUserPreferencesRepository, UserPreferencesRepository>();

            #endregion Preferences

            #region Notifications

            services.AddScoped<INotificationAppService, NotificationAppService>();
            services.AddScoped<INotificationDomainService, NotificationDomainService>();
            services.AddScoped<INotificationRepository, NotificationRepository>();

            #endregion Notifications

            #region Gamification

            services.AddScoped<IGamificationAppService, GamificationAppService>();
            services.AddScoped<IGamificationLevelAppService, GamificationLevelAppService>();
            services.AddScoped<IGamificationDomainService, GamificationDomainService>();
            services.AddScoped<IGamificationLevelDomainService, GamificationLevelDomainService>();
            services.AddScoped<IGamificationRepository, GamificationRepository>();
            services.AddScoped<IGamificationActionRepository, GamificationActionRepository>();
            services.AddScoped<IGamificationLevelRepository, GamificationLevelRepository>();
            services.AddScoped<IUserBadgeRepository, UserBadgeRepository>();

            #endregion Gamification

            #region Interactions

            services.AddScoped<IUserConnectionRepository, UserConnectionRepository>();

            #endregion Interactions

            #region Poll

            services.AddScoped<IPollAppService, PollAppService>();
            services.AddScoped<IPollDomainService, PollDomainService>();
            services.AddScoped<IPollRepository, PollRepository>();

            #endregion Poll

            #region Team

            services.AddScoped<ITeamAppService, TeamAppService>();
            services.AddScoped<ITeamDomainService, TeamDomainService>();
            services.AddScoped<ITeamRepository, TeamRepository>();

            #endregion Team

            #region Jobs

            services.AddScoped<IJobPositionAppService, JobPositionAppService>();
            services.AddScoped<IJobPositionDomainService, JobPositionDomainService>();
            services.AddScoped<IJobPositionRepository, JobPositionRepository>();

            #endregion Jobs

            #region Localization

            services.AddScoped<ILocalizationAppService, LocalizationAppService>();
            services.AddScoped<ILocalizationDomainService, TranslationDomainService>();
            services.AddScoped<ILocalizationRepository, TranslationRepository>();

            #endregion Localization

            #region Study

            services.AddScoped<IStudyAppService, StudyAppService>();
            services.AddScoped<IStudyDomainService, StudyDomainService>();
            services.AddScoped<IStudyCourseRepository, StudyCourseRepository>();
            services.AddScoped<IStudyGroupRepository, StudyGroupRepository>();

            #endregion Study

            #region Giveaway

            services.AddScoped<IGiveawayAppService, GiveawayAppService>();
            services.AddScoped<IGiveawayDomainService, GiveawayDomainService>();
            services.AddScoped<IGiveawayRepository, GiveawayRepository>();

            #endregion Giveaway

            #region ShortUrl

            services.AddScoped<IShortUrlAppService, ShortUrlAppService>();
            services.AddScoped<IShortUrlDomainService, ShortUrlDomainService>();
            services.AddScoped<IShortUrlRepository, ShortUrlRepository>();

            #endregion ShortUrl

            #region Common

            services.AddScoped<IBaseAppServiceCommon, BaseAppServiceCommon>();
            services.AddScoped<IProfileBaseAppServiceCommon, ProfileBaseAppServiceCommon>();

            #endregion Common

            // Infra
            services.AddTransient<INotificationSender, SendGridSlackNotificationService>();

            services.AddTransient<IImageStorageService, CloudinaryService>();

            services.AddScoped<IMongoContext, MongoContext>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}