using System;

namespace LuduStack.Domain.Core.Interfaces
{
    public interface IEntity
    {
        Guid Id { get; set; }

        Guid UserId { get; set; }

        DateTime CreateDate { get; set; }
    }
}