using LuduStack.Web.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LuduStack.Web.Areas.Member.Controllers.Base
{
    [Authorize]
    [Area("member")]
    public class MemberBaseController : SecureBaseController
    {
    }
}