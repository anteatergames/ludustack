using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Domain.Services
{
    public class BrainstormDomainService : IBrainstormDomainService
    {
        protected readonly IBrainstormRepository brainstormRepository;

        public BrainstormDomainService(IBrainstormRepository brainstormRepository)
        {
            this.brainstormRepository = brainstormRepository;
        }

        public BrainstormSession Get(BrainstormSessionType type)
        {
            return brainstormRepository.Get(x => x.Type == type).FirstOrDefault();
        }

        public void AddComment(BrainstormComment model)
        {
            brainstormRepository.AddComment(model);
        }

        public void AddIdea(BrainstormIdea model)
        {
            brainstormRepository.AddIdea(model);
        }

        public void AddVote(BrainstormVote model)
        {
            brainstormRepository.AddVote(model);
        }

        public BrainstormIdea GetIdea(Guid ideaId)
        {
            Task<BrainstormIdea> task = Task.Run(async () => await brainstormRepository.GetIdea(ideaId));

            return task.Result;
        }

        public IEnumerable<BrainstormIdea> GetIdeasBySession(Guid sessionId)
        {
            Task<IEnumerable<BrainstormIdea>> task = Task.Run(async () => await brainstormRepository.GetIdeasBySession(sessionId));

            return task.Result;
        }

        public void UpdateIdea(BrainstormIdea idea)
        {
            brainstormRepository.UpdateIdea(idea);
        }

        public void UpdateVote(BrainstormVote model)
        {
            brainstormRepository.UpdateVote(model);
        }

        public Guid GetUserId(Guid sessionId)
        {
            BrainstormSession obj = brainstormRepository.Get().FirstOrDefault(x => x.Id == sessionId);

            if (obj != null)
            {
                return obj.UserId;
            }
            else
            {
                return Guid.Empty;
            }
        }
    }
}