using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LuduStack.Domain.Interfaces.Repository
{
    public interface IStorePartnershipRepository : IRepository<StorePartnership>
    {
        Task<StorePartnership> GetByPartner(Guid partnerUserId);
    }
}