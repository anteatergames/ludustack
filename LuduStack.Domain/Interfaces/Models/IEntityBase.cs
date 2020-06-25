﻿using System;

namespace LuduStack.Domain.Interfaces.Models
{
    public interface IEntityBase
    {
        Guid Id { get; set; }

        Guid UserId { get; set; }

        DateTime CreateDate { get; set; }
    }
}