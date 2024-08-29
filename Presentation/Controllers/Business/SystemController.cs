using Application.Repositories;
using Application.Services.Interfaces;
using Domain;
using Domain.Dtos.Refrences;
using Domain.Dtos.Shared;
using Domain.Queries.Shared;
using Infrastructure.CQRS.Business.Systems;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.CustomeAttributes;

namespace Presentation.Controllers.Business
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IRepository<Domain.Entities.Business.System> _repository;
        private readonly IUserAccessor _userAccessor;

        public SystemController(IMediator mediator, IRepository<Domain.Entities.Business.System> repository, IUserAccessor userAccessor)
        {
            _mediator = mediator;
            _repository = repository;
            _userAccessor = userAccessor;
        }

        [Route("GetAll")]
        [HttpPost]
        [AccessControl(PermissionsSD.QuerySystem)]
        public async Task<ListActionResult<SystemDetails>> GetAll([FromBody] GridQuery query, CancellationToken cancellationToken)
        {
            var companyId = _userAccessor.GetCompanyId();
            return await _repository.GetAllAsync<SystemDetails>(query, b => b.CompanyId == companyId.Value, cancellationToken);
        }

        [HttpGet("Find/{id}")]
        [AccessControl(PermissionsSD.QuerySystem)]
        public async Task<SystemDetails> Find(Guid id, CancellationToken cancellationToken)
        {
            return await _repository.FirstOrDefaultAsync<SystemDetails>(b => b.Id == id, cancellationToken: cancellationToken);
        }

        [Route("Create")]
        [HttpPost]
        [AccessControl(PermissionsSD.CommandSystem)]
        public async Task<CommandResponse> Create([FromBody] CreateSystemCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }

        [Route("Update")]
        [HttpPut]
        [AccessControl(PermissionsSD.CommandSystem)]
        public async Task<CommandResponse> Update([FromBody] UpdateSystemCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }

        [Route("Delete")]
        [HttpDelete]
        [AccessControl(PermissionsSD.CommandSystem)]
        public async Task<CommandResponse> Delete([FromQuery] DeleteSystemCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }
    }
}
