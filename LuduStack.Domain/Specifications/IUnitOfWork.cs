﻿using System;
using System.Threading.Tasks;

namespace LuduStack.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        bool HasPendingCommands { get; }

        Task<bool> Commit();
    }
}