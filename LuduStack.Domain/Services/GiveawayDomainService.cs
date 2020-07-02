using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LuduStack.Domain.Services
{
    public class GiveawayDomainService : BaseDomainMongoService<Giveaway, IGiveawayRepository>, IGiveawayDomainService
    {
        public GiveawayDomainService(IGiveawayRepository repository) : base(repository)
        {
        }

        public Giveaway GenerateNewGiveaway(Guid userId)
        {
            Giveaway model = new Giveaway
            {
                UserId = userId
            };

            return model;
        }

        public List<GiveawayListItemVo> GetCoursesByUserId(Guid userId)
        {
            List<GiveawayListItemVo> objs = repository.GetGiveawaysByUserId(userId);

            return objs;
        }

        public Giveaway GetGiveawayById(Guid id)
        {
            Task<Giveaway> task = Task.Run(async () => await repository.GetById(id));

            return task.Result;
        }
    }
}