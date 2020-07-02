using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using System;
using System.Collections.Generic;

namespace LuduStack.Domain.Interfaces.Services
{
    public interface IGiveawayDomainService : IDomainService<Giveaway>
    {
        Giveaway GenerateNewGiveaway(Guid userId);

        Giveaway GetGiveawayById(Guid id);

        List<GiveawayListItemVo> GetCoursesByUserId(Guid userId);
    }
}