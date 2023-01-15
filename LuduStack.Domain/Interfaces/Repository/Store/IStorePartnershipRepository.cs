using LuduStack.Domain.Models;
using System;
using System.Threading.Tasks;

namespace LuduStack.Domain.Interfaces.Repository
{
    public interface IStorePartnershipRepository : IRepository<StorePartnership>
    {
        Task<StorePartnership> GetByPartner(Guid partnerUserId);
    }
}