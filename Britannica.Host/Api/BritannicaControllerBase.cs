using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Britannica.Host.Api
{
    [Authorize]
    public class BritannicaControllerBase : ApiControllerBase
    {
        protected IActionResult Json(object response) => new JsonResult(response);
    }
}
