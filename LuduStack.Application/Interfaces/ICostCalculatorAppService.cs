using LuduStack.Application.ViewModels.CostCalculator;
using LuduStack.Domain.ValueObjects;
using System;
using System.Threading.Tasks;

namespace LuduStack.Application.Interfaces
{
    public interface ICostCalculatorAppService
    {
        Task<OperationResultVo<CostsViewModel>> GetRates(Guid currentUserId);
    }
}