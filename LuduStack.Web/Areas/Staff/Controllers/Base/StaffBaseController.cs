using LuduStack.Web.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LuduStack.Web.Areas.Staff.Controllers.Base
{
    [Authorize]
    [Area("staff")]
    public class StaffBaseController : SecureBaseController
    {
    }
}