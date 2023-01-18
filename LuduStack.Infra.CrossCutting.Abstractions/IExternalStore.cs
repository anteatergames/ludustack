using LuduStack.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LuduStack.Infra.CrossCutting.Abstractions
{
    public interface IExternalStore
    {
        Task<List<Product>> GetProductsAsync();

        Task<List<Order>> GetOrdersAsync();
    }
}