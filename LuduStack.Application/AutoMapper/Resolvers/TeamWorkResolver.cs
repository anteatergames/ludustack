using AutoMapper;
using LuduStack.Application.ViewModels.Team;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuduStack.Application.AutoMapper.Resolvers
{
    public class TeamWorkToDomainResolver : IValueResolver<TeamMemberViewModel, TeamMember, string>
    {
        public string Resolve(TeamMemberViewModel source, TeamMember destination, string destMember, ResolutionContext context)
        {
            string result = string.Empty;

            if (source.Works == null || !source.Works.Any())
            {
                return result;
            }

            result = string.Join('|', source.Works);

            return result;
        }
    }

    public class TeamWorkFromDomainResolver : IValueResolver<TeamMember, TeamMemberViewModel, List<WorkType>>
    {
        public List<WorkType> Resolve(TeamMember source, TeamMemberViewModel destination, List<WorkType> destMember, ResolutionContext context)
        {
            string[] platforms = (source.Work ?? string.Empty).Split(new[] { '|' });

            IEnumerable<WorkType> platformsConverted = platforms.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => (WorkType)Enum.Parse(typeof(WorkType), x));

            return platformsConverted.ToList();
        }
    }
}