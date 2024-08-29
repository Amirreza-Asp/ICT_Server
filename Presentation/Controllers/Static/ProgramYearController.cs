using Application.Repositories;
using Domain;
using Domain.Dtos.Shared;
using Domain.Dtos.Static;
using Domain.Entities.Static;
using Domain.Queries.Shared;
using Infrastructure.CQRS.Static.ProgramYears;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.CustomeAttributes;

namespace Presentation.Controllers.Static
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProgramYearController : ControllerBase
    {
        private readonly IRepository<ProgramYear> _repo;
        private readonly IMediator _mediator;

        public ProgramYearController(IMediator mediator, IRepository<ProgramYear> repo)
        {
            _mediator = mediator;
            _repo = repo;
        }

        [Route("GetAll")]
        [HttpPost]
        public async Task<ListActionResult<ProgramYearSummary>> GetAll([FromBody] GridQuery query, CancellationToken cancellationToken)
        {
            return await _repo.GetAllAsync<ProgramYearSummary>(query, cancellationToken);
        }

        [Route("DropDown")]
        [HttpGet]
        public async Task<List<ProgramYearSummary>> DropDown(CancellationToken cancellationToken)
        {
            return await _repo.GetAllAsync<ProgramYearSummary>(cancellationToken: cancellationToken);
        }

        [Route("Find")]
        [HttpGet]
        public async Task<ProgramYearSummary> Find([FromQuery] Guid id, CancellationToken cancellationToken) =>
            await _repo.FirstOrDefaultAsync<ProgramYearSummary>(b => b.Id == id, cancellationToken: cancellationToken);


        [Route("Create")]
        [HttpPost]
        [AccessControl(PermissionsSD.CommandProgramYear)]
        public async Task<CommandResponse> Create([FromBody] CreateProgramYearCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command);
        }

        [Route("Update")]
        [HttpPut]
        [AccessControl(PermissionsSD.CommandProgramYear)]
        public async Task<CommandResponse> Update([FromBody] UpdateProgramYearCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command);
        }

        [Route("Remove")]
        [HttpDelete]
        [AccessControl(PermissionsSD.CommandProgramYear)]
        public async Task<CommandResponse> Remove([FromQuery] RemoveProgramYearCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command);
    }
}
