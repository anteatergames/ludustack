using AutoMapper;
using LuduStack.Application.ViewModels;
using LuduStack.Application.ViewModels.User;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using System.Collections.Generic;
using System.Linq;

namespace LuduStack.Application.AutoMapper.MappingActions
{
    public class AddOrUpdateProfileExternalLinks : IMappingAction<ProfileViewModel, UserProfile>
    {
        public void Process(ProfileViewModel source, UserProfile destination, ResolutionContext context)
        {
            List<ExternalLinkVo> destinationExternalLinks = new List<ExternalLinkVo>();

            foreach (ExternalLinkBaseViewModel externalLink in source.ExternalLinks)
            {
                ExternalLinkVo existing = destination.ExternalLinks.FirstOrDefault(x => x.Provider == externalLink.Provider);
                if (existing == null)
                {
                    ExternalLinkVo newExternalLink = context.Mapper.Map<ExternalLinkVo>(externalLink);
                    destinationExternalLinks.Add(newExternalLink);
                }
                else
                {
                    context.Mapper.Map(externalLink, existing);
                    destinationExternalLinks.Add(existing);
                }
            }

            destination.ExternalLinks = destinationExternalLinks;
        }
    }
}