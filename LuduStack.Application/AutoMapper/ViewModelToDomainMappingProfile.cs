using LuduStack.Application.AutoMapper.MappingActions;
using LuduStack.Application.AutoMapper.Resolvers;
using LuduStack.Application.ViewModels;
using LuduStack.Application.ViewModels.Brainstorm;
using LuduStack.Application.ViewModels.Content;
using LuduStack.Application.ViewModels.FeaturedContent;
using LuduStack.Application.ViewModels.Game;
using LuduStack.Application.ViewModels.Gamification;
using LuduStack.Application.ViewModels.Giveaway;
using LuduStack.Application.ViewModels.Jobs;
using LuduStack.Application.ViewModels.Localization;
using LuduStack.Application.ViewModels.Notification;
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

            CreateMap<FeaturedContentViewModel, Domain.Models.FeaturedContent>();

            CreateMap<UserPreferencesViewModel, Domain.Models.UserPreferences>()
                .ForMember(dest => dest.ContentLanguages, opt => opt.MapFrom<UserLanguagesToDomainResolver>());

            CreateMap<NotificationItemViewModel, Domain.Models.Notification>();

            CreateMap<ExternalLinkBaseViewModel, ExternalLinkVo>();

            CreateMap<CommentViewModel, Domain.Models.UserContentComment>();

            CreateMap<CommentViewModel, Domain.Models.BrainstormComment>();

            #endregion General

            #region Game

            CreateMap<GameViewModel, Domain.Models.Game>()
                    .ForMember(dest => dest.DeveloperName, opt => opt.MapFrom(src => src.AuthorName))
                    .ForMember(dest => dest.Platforms, opt => opt.MapFrom<GamePlatformToDomainResolver>())
                    .ForMember(dest => dest.ExternalLinks, opt => opt.Ignore())
                    .AfterMap<AddOrUpdateGameExternalLinks>();

            #endregion Game

            #region Profile

            CreateMap<ProfileViewModel, Domain.Models.UserProfile>()
                .ForMember(dest => dest.Followers, opt => opt.Ignore())
                .AfterMap<AddOrUpdateProfileExternalLinks>();

            #endregion Profile

            #region Content

            CreateMap<UserContentViewModel, Domain.Models.UserContent>();

            #endregion Content

            #region Brainstorm

            CreateMap<BrainstormSessionViewModel, Domain.Models.BrainstormSession>();
            CreateMap<BrainstormIdeaViewModel, Domain.Models.BrainstormIdea>();
            CreateMap<BrainstormVoteViewModel, Domain.Models.BrainstormVote>();
            CreateMap<BrainstormCommentViewModel, Domain.Models.BrainstormComment>();

            #endregion Brainstorm

            #region Gamification

            CreateMap<UserBadgeViewModel, Domain.Models.UserBadge>();
            CreateMap<GamificationLevelViewModel, Domain.Models.GamificationLevel>();

            #endregion Gamification

            #region Interactions

            CreateMap<GameFollowViewModel, Domain.Models.GameFollow>();

            CreateMap<UserFollowViewModel, Domain.Models.UserFollow>();

            CreateMap<UserConnectionViewModel, Domain.Models.UserConnection>();

            #endregion Interactions

            #region Team

            CreateMap<TeamViewModel, Domain.Models.Team>()
                .ForMember(dest => dest.Members, opt => opt.Ignore())
                .AfterMap<AddOrUpdateTeamMembers>();
            CreateMap<TeamMemberViewModel, Domain.Models.TeamMember>()
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

            CreateMap<CourseViewModel, Domain.Models.StudyCourse>()
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
            CreateMap<GiveawayViewModel, GiveawayBasicInfo>();
            CreateMap<GiveawayPrizeViewModel, GiveawayPrize>();
            CreateMap<GiveawayEntryOptionViewModel, GiveawayEntryOption>();
            CreateMap<GiveawayParticipantViewModel, GiveawayParticipant>();
            CreateMap<GiveawayEntryViewModel, GiveawayEntry>();

            #endregion Giveaway

            #region ShortUrl

            CreateMap<ShortUrlViewModel, Domain.Models.ShortUrl>();

            #endregion ShortUrl
        }
    }
}