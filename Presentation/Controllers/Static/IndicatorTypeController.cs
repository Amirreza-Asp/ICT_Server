using Application.Repositories;
using Domain;
using Domain.Dtos.Shared;
using Domain.Entities.Static;
using Domain.Queries.Shared;
using Infrastructure.CQRS.Static.IndicatorTypes;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.CustomeAttributes;

namespace Presentation.Controllers.Static
{
    [Route("api/[controller]")]
    [ApiController]
    public class IndicatorTypeController : ControllerBase
    {
        private readonly IRepository<IndicatorType> _intRepo;
        private readonly IMediator _mediator;

        public IndicatorTypeController(IRepository<IndicatorType> intRepo, IMediator mediator)
        {
            _intRepo = intRepo;
            _mediator = mediator;
        }

        [Route("SelectList")]
        [HttpGet]
        public async Task<List<SelectSummary>> GetSelectList(CancellationToken cancellationToken)
        {
            return await _intRepo.GetAllAsync<SelectSummary>(cancellationToken: cancellationToken);
        }

        [Route("GetAll")]
        [HttpPost]
        public async Task<ListActionResult<IndicatorType>> GetAll([FromBody] GridQuery query, CancellationToken cancellationToken)
        {
            return await _intRepo.GetAllAsync<IndicatorType>(query, cancellationToken);
        }

        [Route("Find")]
        [HttpGet]
        public async Task<IndicatorType> Find([FromQuery] Guid id, CancellationToken cancellationToken) =>
            await _intRepo.FirstOrDefaultAsync<IndicatorType>(b => b.Id == id, cancellationToken: cancellationToken);

        [Route("Create")]
        [HttpPost]
        [AccessControl(PermissionsSD.CommandIndicatorType)]
        public async Task<CommandResponse> Create([FromBody] CreateIndicatorTypeCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command);
        }

        [Route("Update")]
        [HttpPut]
        [AccessControl(PermissionsSD.CommandIndicatorType)]
        public async Task<CommandResponse> Update([FromBody] UpdateIndicatorTypeCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command);
        }

        [Route("Remove")]
        [HttpDelete]
        [AccessControl(PermissionsSD.CommandIndicatorType)]
        public async Task<CommandResponse> Remove([FromQuery] RemoveIndicatorTypeCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command);
    }
}
