using Britannica.Application.Interactors;
using Britannica.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace Britannica.Host.Api
{
    [Route("api/Passengers")]
    [ApiExplorerSettings(GroupName = "Passengers")]
    public class PassengersController : BritannicaControllerBase
    {
        private readonly IMediator _mediator;
        public PassengersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<PassengerEntity>), StatusCodes.Status200OK)]
        public async Task<ActionResult> Get(
            [FromQuery, Required] int pageIndex,
            [FromQuery, Required] int totalPages,
            CancellationToken cancellationToken)
             => Ok(await _mediator.Send(new GetPassengersRequest(pageIndex, totalPages), cancellationToken));

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PassengerEntity), StatusCodes.Status200OK)]
        public async Task<ActionResult> Get(
            [FromRoute, Required] int id,
            CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new GetPassengerRequest(id), cancellationToken));

        [HttpGet(nameof(Flights) + "/{id}")]
        [ProducesResponseType(typeof(PassengerEntity), StatusCodes.Status200OK)]
        public async Task<ActionResult> Flights(
            [FromRoute, Required] int id,
            CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new GetPassengerFlightRequest(id), cancellationToken));

        [HttpPost(nameof(CheckIn))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult> CheckIn(
            [FromBody] CheckInRequest request,
            CancellationToken cancellationToken)
        {
            var createdEntity = await _mediator.Send(request, cancellationToken);
            var queryParams = new { createdEntity.Id };
            var fullCreatedEntityUrl = Url.Action(nameof(Flights), "Passengers", queryParams, Request.Scheme);
            return Created(fullCreatedEntityUrl, createdEntity);
        }
    }
}