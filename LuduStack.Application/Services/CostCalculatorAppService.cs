using AutoMapper;
using AutoMapper.QueryableExtensions;
using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.CostCalculator;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Messaging;
using LuduStack.Domain.Messaging.Queries.BillRate;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Application.Services
{
    public class CostCalculatorAppService : ICostCalculatorAppService
    {
        protected readonly IMediatorHandler mediator;

        protected readonly IMapper mapper;

        public CostCalculatorAppService(IMediatorHandler mediator, IMapper mapper)
        {
            this.mediator = mediator;
            this.mapper = mapper;
        }

        public async Task<OperationResultVo<CostsViewModel>> GetRates(Guid currentUserId)
        {
            try
            {
                var rates = (await mediator.Query<GetBillRateCostsQuery, CostCalculatorVo>(new GetBillRateCostsQuery()));

                var vm = mapper.Map<CostsViewModel>(rates);

                return new OperationResultVo<CostsViewModel>(vm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<CostsViewModel>(ex.Message);
            }
        }
    }
}
