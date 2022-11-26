using Britannica.Host.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Britannica.Host.Api
{

    [ApiController]
    [ApiExceptionFilter]
    [ProducesResponseType(typeof(AppValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(AppNotFoundProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BusinessRuleProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public abstract class ApiControllerBase : ControllerBase
    {
        [NonAction]
        public virtual string UrlLink(string routeName, object values) => Url.Link(routeName, values);
    }
}

