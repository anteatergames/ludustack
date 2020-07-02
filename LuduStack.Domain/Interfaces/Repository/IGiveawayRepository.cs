using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using System;
using System.Collections.Generic;

namespace LuduStack.Domain.Interfaces.Repository
{
    public interface IGiveawayRepository : IRepository<Giveaway>
    {
        List<GiveawayListItemVo> GetGiveawaysByUserId(Guid userId);
    }
}