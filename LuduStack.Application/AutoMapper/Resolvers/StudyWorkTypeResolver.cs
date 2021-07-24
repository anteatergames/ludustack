using AutoMapper;
using LuduStack.Application.ViewModels.Study;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuduStack.Application.AutoMapper.Resolvers
{
    public class StudyCourseWorkTypeToDomainResolver : IValueResolver<CourseViewModel, StudyCourse, string>
    {
        public string Resolve(CourseViewModel source, StudyCourse destination, string destMember, ResolutionContext context)
        {
            string result = string.Empty;

            if (source.SkillSet == null || !source.SkillSet.Any())
            {
                return result;
            }

            result = string.Join('|', source.SkillSet);

            return result;
        }
    }

    public class StudyCourseWorkTypeFromDomainResolver : IValueResolver<StudyCourse, CourseViewModel, List<WorkType>>
    {
        public List<WorkType> Resolve(StudyCourse source, CourseViewModel destination, List<WorkType> destMember, ResolutionContext context)
        {
            string[] platforms = (source.SkillSet ?? string.Empty).Split(new[] { '|' });

            IEnumerable<WorkType> platformsConverted = platforms.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => (WorkType)Enum.Parse(typeof(WorkType), x));

            return platformsConverted.ToList();
        }
    }
}