using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LuduStack.Domain.Interfaces.Services
{
    public interface IUserContentDomainService : IDomainService<UserContent>
    {
        void Comment(UserContentComment model);

        DomainOperationVo<UserContentRating> Rate(Guid userId, Guid id, decimal scoreDecimal);
    }
}