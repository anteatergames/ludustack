using AutoMapper;
using LuduStack.Application.AutoMapper.Resolvers;
using LuduStack.Application.ViewModels;
using LuduStack.Application.ViewModels.BillRate;
using LuduStack.Application.ViewModels.Brainstorm;
using LuduStack.Application.ViewModels.Comics;
using LuduStack.Application.ViewModels.Content;
using LuduStack.Application.ViewModels.CostCalculator;
using LuduStack.Application.ViewModels.FeaturedContent;
using LuduStack.Application.ViewModels.Forum;
using LuduStack.Application.ViewModels.Game;
using LuduStack.Application.ViewModels.GameIdea;
using LuduStack.Application.ViewModels.GameJam;
using LuduStack.Application.ViewModels.Gamification;
using LuduStack.Application.ViewModels.Giveaway;
using LuduStack.Application.ViewModels.Jobs;
using LuduStack.Application.ViewModels.Localization;
using LuduStack.Application.ViewModels.Notification;
using LuduStack.Application.ViewModels.PlatformSetting;
using LuduStack.Application.ViewModels.Search;
using LuduStack.Application.ViewModels.ShortUrl;
using LuduStack.Application.ViewModels.Study;
using LuduStack.Application.ViewModels.Team;
using LuduStack.Application.ViewModels.User;
using LuduStack.Application.ViewModels.UserPreferences;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using System.Linq;

namespace LuduStack.Application.AutoMapper
{
    internal class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {
            #region General

            CreateMap<Game, SelectListItemVo>()
                    .ForMember(x => x.Value, opt => opt.MapFrom(x => x.Id.ToString()))
                    .ForMember(x => x.Text, opt => opt.MapFrom(x => x.Title));

            CreateMap<FeaturedContent, FeaturedContentViewModel>();

            CreateMap<UserPreferences, UserPreferencesViewModel>()
                .ForMember(dest => dest.Languages, opt => opt.MapFrom<UserLanguagesFromDomainResolver>());

            CreateMap<UserContentComment, CommentViewModel>();

            CreateMap<BrainstormComment, CommentViewModel>();

            CreateMap<Notification, NotificationItemViewModel>();

            #endregion General

            #region Game

            CreateMap<Game, GameViewModel>()
                    .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.DeveloperName))
                    .ForMember(dest => dest.Platforms, opt => opt.MapFrom<GamePlatformFromDomainResolver>());
            CreateMap<Game, GameListItemViewModel>();

            CreateMap<ExternalLinkVo, ExternalLinkBaseViewModel>();

            #endregion Game

            #region Profile

            CreateMap<UserProfile, ProfileViewModel>()
                    .ForMember(x => x.Counters, opt => opt.Ignore())
                    .ForMember(x => x.IndieXp, opt => opt.Ignore());

            CreateMap<UserProfile, UserProfileEssentialVo>();

            CreateMap<UserProfileEssentialVo, ProfileViewModel>()
                    .ForMember(x => x.Counters, opt => opt.Ignore())
                    .ForMember(x => x.IndieXp, opt => opt.Ignore());

            CreateMap<ExternalLinkVo, ExternalLinkBaseViewModel>();

            #endregion Profile

            #region Content

            CreateMap<UserContent, UserContentViewModel>()
                .ForMember(x => x.Likes, opt => opt.MapFrom(x => x.Likes.Select(y => y.UserId)))
                .ForMember(x => x.LikeCount, opt => opt.MapFrom(x => x.Likes.Count));

            CreateMap<UserContent, UserContentToBeFeaturedViewModel>();

            #endregion Content

            #region Brainstorm

            CreateMap<BrainstormSession, BrainstormSessionViewModel>();
            CreateMap<BrainstormIdea, BrainstormIdeaViewModel>();
            CreateMap<BrainstormComment, BrainstormCommentViewModel>();

            #endregion Brainstorm

            #region Gamification

            CreateMap<UserBadge, UserBadgeViewModel>();
            CreateMap<Gamification, RankingViewModel>();
            CreateMap<GamificationLevel, GamificationLevelViewModel>();

            #endregion Gamification

            #region Interaction

            CreateMap<GameFollow, GameFollowViewModel>();
            CreateMap<UserFollow, UserFollowViewModel>();
            CreateMap<UserConnection, UserConnectionViewModel>();

            #endregion Interaction

            #region Search

            CreateMap<UserContentSearchVo, UserContentSearchViewModel>();

            #endregion Search

            #region Team

            CreateMap<Team, TeamViewModel>();
            CreateMap<TeamMember, TeamMemberViewModel>()
                    .ForMember(dest => dest.Works, opt => opt.MapFrom<TeamWorkFromDomainResolver>());

            CreateMap<UserProfile, ProfileSearchViewModel>();

            #endregion Team

            #region Jobs

            CreateMap<JobPosition, JobPositionViewModel>();
            CreateMap<JobApplicant, JobApplicantViewModel>();

            #endregion Jobs

            #region Localization

            CreateMap<Localization, LocalizationViewModel>()
                .ForPath(x => x.Game.Id, opt => opt.MapFrom(x => x.GameId));
            CreateMap<Localization, TranslationStatsViewModel>()
                .ForPath(x => x.Game.Id, opt => opt.MapFrom(x => x.GameId));
            CreateMap<LocalizationTerm, LocalizationTermViewModel>();
            CreateMap<LocalizationEntry, LocalizationEntryViewModel>();

            #endregion Localization

            #region Study

            CreateMap<StudyCourse, CourseViewModel>()
                    .ForMember(dest => dest.SkillSet, opt => opt.MapFrom<StudyCourseWorkTypeFromDomainResolver>());

            CreateMap<CourseMember, CourseMemberViewModel>();
            CreateMap<StudyGroup, StudyGroupViewModel>();
            CreateMap<StudyPlan, StudyPlanViewModel>();
            CreateMap<StudyActivity, StudyActivityViewModel>();

            #endregion Study

            #region Giveaway

            CreateMap<Giveaway, GiveawayViewModel>();
            CreateMap<GiveawayBasicInfoVo, GiveawayViewModel>();
            CreateMap<GiveawayBasicInfoVo, GiveawayParticipationViewModel>();
            CreateMap<GiveawayPrize, GiveawayPrizeViewModel>();
            CreateMap<GiveawayEntryOption, GiveawayEntryOptionViewModel>();
            CreateMap<GiveawayParticipant, GiveawayParticipantViewModel>()
                .ForMember(dest => dest.EmailVerified, opt => opt.MapFrom(src => src.Entries.Any(x => x.Type == GiveawayEntryType.EmailConfirmed)));
            CreateMap<GiveawayEntry, GiveawayEntryViewModel>();

            #endregion Giveaway

            #region ShortUrl

            CreateMap<ShortUrl, ShortUrlViewModel>();

            #endregion ShortUrl

            #region Comics

            CreateMap<UserContent, ComicStripViewModel>()
                    .ForMember(x => x.Likes, opt => opt.MapFrom(x => x.Likes.Select(y => y.UserId)))
                    .ForMember(x => x.LikeCount, opt => opt.MapFrom(x => x.Likes.Count));

            #endregion Comics

            #region BillRate

            CreateMap<BillRate, BillRateViewModel>();
            CreateMap<CostCalculatorVo, CostsViewModel>();
            CreateMap<RateBaseVo, RateBaseViewModel>();
            CreateMap<ResultRateVo, ResultRateBaseViewModel>();
            CreateMap<VisualRateVo, VisualRateCostsViewModel>();
            CreateMap<AudioRateVo, AudioRateCostsViewModel>();
            CreateMap<CodeRateVo, CodeRateCostsViewModel>();
            CreateMap<TextRateVo, TextRateCostsViewModel>();

            #endregion BillRate

            #region Forum

            CreateMap<ForumGroup, ForumGroupViewModel>();
            CreateMap<ForumCategory, ForumCategoryViewModel>();
            CreateMap<ForumCategory, ForumCategoryListItemVo>();
            CreateMap<ForumPost, ForumPostViewModel>();

            #endregion Forum

            #region PlatformSetting

            CreateMap<PlatformSetting, PlatformSettingViewModel>();

            #endregion PlatformSetting

            #region GameJam

            CreateMap<GameJam, GameJamViewModel>();
            CreateMap<GameJamListItem, GameJamViewModel>();
            CreateMap<GameJamEntry, GameJamEntryViewModel>();
            CreateMap<GameJamCriteria, GameJamCriteriaViewModel>();
            CreateMap<GameJamSponsor, GameJamSponsorViewModel>();
            CreateMap<GameJamVote, GameJamVoteViewModel>();
            CreateMap<GameJamJudge, GameJamJudgeViewModel>();
            CreateMap<GameJamTeamMember, GameJamTeamMemberViewModel>();
            CreateMap<GameJamCriteriaResult, GameJamCriteriaResultViewModel>();

            #endregion GameJam

            #region GameIdea

            CreateMap<GameIdea, GameIdeaViewModel>();

            #endregion GameIdea
        }
    }
}