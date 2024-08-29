using Application.Repositories;
using Application.Services.Interfaces;
using Domain;
using Domain.Dtos.Programs;
using Domain.Dtos.Shared;
using Domain.Entities.Business;
using Domain.Queries.Shared;
using Infrastructure.CQRS.Business.Perspectives;
using Infrastructure.CQRS.Business.Programs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.CustomeAttributes;

namespace Presentation.Controllers.Business
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProgramController : ControllerBase
    {
        private readonly IRepository<Domain.Entities.Business.Program> _programRepo;
        private readonly IMediator _mediator;
        private readonly IUserAccessor _userAccessor;
        private readonly IRepository<Perspective> _perspectiveRepository;


        public ProgramController(IRepository<Domain.Entities.Business.Program> programRepo, IMediator mediator, IUserAccessor userAccessor, IRepository<Perspective> perspectiveRepository)
        {
            _programRepo = programRepo;
            _mediator = mediator;
            _userAccessor = userAccessor;
            _perspectiveRepository = perspectiveRepository;
        }

        [HttpPost]
        [Route("GetAll")]
        [AccessControl(PermissionsSD.QueryProgram)]
        public async Task<ListActionResult<ProgramSummary>> GetAll([FromBody] GridQuery query, CancellationToken cancellationToken)
        {
            var companyId = _userAccessor.GetCompanyId();
            return await _programRepo.GetAllAsync<ProgramSummary>(query, b => b.CompanyId == companyId, cancellationToken);
        }

        [HttpGet]
        [Route("Find")]
        [AccessControl(PermissionsSD.QueryProgram)]
        public async Task<ProgramDetails> Find([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            return await _programRepo.FirstOrDefaultAsync<ProgramDetails>(b => b.Id == id, cancellationToken);
        }

        [HttpPost]
        [Route("Create")]
        [AccessControl(PermissionsSD.CommandProgram)]
        public async Task<CommandResponse> Create([FromBody] CreateProgramCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);

        [HttpPut]
        [Route("Update")]
        [AccessControl(PermissionsSD.CommandProgram)]
        public async Task<CommandResponse> Update([FromBody] UpdateProgramCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);

        [HttpPut]
        [Route("ChangeActive")]
        [AccessControl(PermissionsSD.CommandProgram)]
        public async Task<CommandResponse> ChangeActive([FromBody] ChangeProgramActiveCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);

        [HttpDelete]
        [Route("Delete")]
        [AccessControl(PermissionsSD.CommandProgram)]
        public async Task<CommandResponse> Delete([FromQuery] DeleteProgramCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);

        [HttpGet]
        [Route("Perspective")]
        [AccessControl(PermissionsSD.SeePerspective)]
        public async Task<PerspectiveSummary> Perspective(CancellationToken cancellationToken)
        {
            var companyId = _userAccessor.GetCompanyId().Value;
            return
                await _perspectiveRepository
                    .FirstOrDefaultAsync<PerspectiveSummary>(b => b.Program.IsActive && b.Program.CompanyId == companyId);
        }

        [HttpPost]
        [Route("UpsertPerspective")]
        [AccessControl(PermissionsSD.UpsertPerspective)]
        public async Task<CommandResponse> UpsertPerspective([FromBody] UpsertPerspectiveCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);
    }
}
