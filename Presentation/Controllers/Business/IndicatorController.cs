using Domain.Dtos.Shared;
using Infrastructure.CQRS.Business.Indicators;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.Business
{
    [Route("api/[controller]")]
    [ApiController]
    public class IndicatorController : ControllerBase
    {
        private readonly IMediator _mediator;

        public IndicatorController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("AddProgress")]
        [HttpPost]
        public async Task<CommandResponse> AddProgress([FromBody] AddIndicatorProgressCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }
    }
}
