using LuduStack.Web.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LuduStack.Web.Areas.Staff.Controllers.Base
{
    [Authorize(Roles = "Administrator")]
    [Area("staff")]
    public class StaffBaseController : SecureBaseController
    {
    }
}