using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Extensions;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LuduStack.Domain.Services
{
    public class UserContentDomainService : BaseDomainMongoService<UserContent, IUserContentRepository>, IUserContentDomainService
    {
        public UserContentDomainService(IUserContentRepository repository) : base(repository)
        {
        }
    }
}