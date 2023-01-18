using LuduStack.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LuduStack.Domain.Interfaces.Repository
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<List<Product>> GetByOwner(Guid ownerUserId);
    }
}