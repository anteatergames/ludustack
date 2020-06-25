using System;

namespace LuduStack.Application.Interfaces
{
    public interface IPermissionControl<in TViewModel>
    {
        void SetPermissions(Guid currentUserId, TViewModel vm);
    }
}