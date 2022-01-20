using LuduStack.Application.AutoMapper.MappingActions;
using LuduStack.Application.AutoMapper.Resolvers;
using LuduStack.Application.ViewModels;
using LuduStack.Application.ViewModels.BillRate;
using LuduStack.Application.ViewModels.Brainstorm;
using LuduStack.Application.ViewModels.Comics;
using LuduStack.Application.ViewModels.Content;
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
using LuduStack.Application.ViewModels.Poll;
using LuduStack.Application.ViewModels.ShortUrl;
using LuduStack.Application.ViewModels.Study;
using LuduStack.Application.ViewModels.Team;
using LuduStack.Application.ViewModels.User;
using LuduStack.Application.ViewModels.UserPreferences;
using LuduStack.Domain.Core.Models;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using System;
using Profile = AutoMapper.Profile;

namespace LuduStack.Application.AutoMapper
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            #region General

            CreateMap<BaseViewModel, Entity>()
                .ForMember(dest => dest.CreateDate, opt => opt.Condition(x => x.CreateDate != DateTime.MinValue));

            CreateMap<FeaturedContentViewModel, FeaturedContent>();

            CreateMap<UserPreferencesViewModel, UserPreferences>()
                .ForMember(dest => dest.ContentLanguages, opt => opt.MapFrom<UserLanguagesToDomainResolver>());

            CreateMap<NotificationItemViewModel, Notification>();

            CreateMap<ExternalLinkBaseViewModel, ExternalLinkVo>();

            CreateMap<CommentViewModel, UserContentComment>();

            CreateMap<CommentViewModel, BrainstormComment>();

            #endregion General

            #region Game

            CreateMap<GameViewModel, Game>()
                    .ForMember(dest => dest.DeveloperName, opt => opt.MapFrom(src => src.AuthorName))
                    .ForMember(dest => dest.Platforms, opt => opt.MapFrom<GamePlatformToDomainResolver>())
                    .ForMember(dest => dest.ExternalLinks, opt => opt.Ignore())
                    .AfterMap<AddOrUpdateGameExternalLinks>();

            #endregion Game

            #region Profile

            CreateMap<ProfileViewModel, UserProfile>()
                .ForMember(dest => dest.Followers, opt => opt.Ignore())
                .AfterMap<AddOrUpdateProfileExternalLinks>();

            #endregion Profile

            #region Content

            CreateMap<UserContentViewModel, UserContent>()
                .ForMember(dest => dest.Likes, opt => opt.Ignore())
                .ForMember(dest => dest.Comments, opt => opt.Ignore());

            #endregion Content

            #region Poll

            CreateMap<PollViewModel, Poll>()
                .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.PollOptions));
            CreateMap<PollOptionViewModel, PollOption>();

            #endregion Poll

            #region Brainstorm

            CreateMap<BrainstormSessionViewModel, BrainstormSession>();
            CreateMap<BrainstormIdeaViewModel, BrainstormIdea>();
            CreateMap<BrainstormCommentViewModel, BrainstormComment>();

            #endregion Brainstorm

            #region Gamification

            CreateMap<UserBadgeViewModel, UserBadge>();
            CreateMap<GamificationLevelViewModel, GamificationLevel>();

            #endregion Gamification

            #region Interactions

            CreateMap<GameFollowViewModel, GameFollow>();

            CreateMap<UserFollowViewModel, UserFollow>();

            CreateMap<UserConnectionViewModel, UserConnection>();

            #endregion Interactions

            #region Team

            CreateMap<TeamViewModel, Team>()
                .ForMember(dest => dest.Members, opt => opt.Ignore())
                .AfterMap<AddOrUpdateTeamMembers>();
            CreateMap<TeamMemberViewModel, TeamMember>()
                    .ForMember(dest => dest.Work, opt => opt.MapFrom<TeamWorkToDomainResolver>());

            #endregion Team

            #region Jobs

            CreateMap<JobPositionViewModel, JobPosition>()
                .ForMember(dest => dest.Applicants, opt => opt.Ignore());
            CreateMap<JobApplicantViewModel, JobApplicant>();

            #endregion Jobs

            #region Localization

            CreateMap<LocalizationViewModel, Localization>()
                .ForMember(dest => dest.Terms, opt => opt.Ignore())
                .ForMember(dest => dest.Entries, opt => opt.Ignore());
            CreateMap<LocalizationTermViewModel, LocalizationTerm>();
            CreateMap<LocalizationEntryViewModel, LocalizationEntry>();

            #endregion Localization

            #region Study

            CreateMap<CourseViewModel, StudyCourse>()
                .ForMember(dest => dest.Plans, opt => opt.Ignore())
                .ForMember(dest => dest.SkillSet, opt => opt.MapFrom<StudyCourseWorkTypeToDomainResolver>());

            CreateMap<CourseMemberViewModel, CourseMember>();
            CreateMap<StudyGroupViewModel, StudyGroup>();
            CreateMap<StudyPlanViewModel, StudyPlan>();
            CreateMap<StudyActivityViewModel, StudyActivity>();

            #endregion Study

            #region Giveaway

            CreateMap<GiveawayViewModel, Giveaway>()
                .ForMember(dest => dest.Participants, opt => opt.Ignore());
            CreateMap<GiveawayViewModel, GiveawayBasicInfoVo>();
            CreateMap<GiveawayPrizeViewModel, GiveawayPrize>();
            CreateMap<GiveawayEntryOptionViewModel, GiveawayEntryOption>();
            CreateMap<GiveawayParticipantViewModel, GiveawayParticipant>();
            CreateMap<GiveawayEntryViewModel, GiveawayEntry>();

            #endregion Giveaway

            #region ShortUrl

            CreateMap<ShortUrlViewModel, ShortUrl>();

            #endregion ShortUrl

            #region Comics

            CreateMap<ComicStripViewModel, UserContent>()
                .ForMember(dest => dest.Likes, opt => opt.Ignore())
                .ForMember(dest => dest.Comments, opt => opt.Ignore());

            #endregion Comics

            #region BillRate

            CreateMap<BillRateViewModel, BillRate>();

            #endregion BillRate

            #region Forum

            CreateMap<ForumGroupViewModel, ForumGroup>();
            CreateMap<ForumCategoryViewModel, ForumCategory>();
            CreateMap<ForumPostViewModel, ForumPost>();

            #endregion Forum

            #region PlatformSetting

            CreateMap<PlatformSettingViewModel, PlatformSetting>();

            #endregion PlatformSetting

            #region GameJam

            CreateMap<GameJamViewModel, GameJam>();
            CreateMap<GameJamEntryViewModel, GameJamEntry>();
            CreateMap<GameJamCriteriaViewModel, GameJamCriteria>();
            CreateMap<GameJamSponsorViewModel, GameJamSponsor>();
            CreateMap<GameJamVoteViewModel, GameJamVote>();
            CreateMap<GameJamJudgeViewModel, GameJamJudge>();
            CreateMap<GameJamTeamMemberViewModel, GameJamTeamMember>();
            CreateMap<GameJamCriteriaResultViewModel, GameJamCriteriaResult>();

            #endregion GameJam

            #region GameIdea

            CreateMap<GameIdeaViewModel, GameIdea>();

            #endregion GameIdea
        }
    }
}