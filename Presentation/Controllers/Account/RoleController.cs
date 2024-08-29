using Application.Repositories;
using Application.Services.Interfaces;
using Domain;
using Domain.Dtos.Account.Roles;
using Domain.Dtos.Shared;
using Domain.Entities.Account;
using Domain.Queries.Shared;
using Infrastructure.CQRS.Account.Roles;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.CustomeAttributes;

namespace Presentation.Controllers.Account
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRepository<Role> _roleRepository;
        private readonly IMediator _mediator;
        private readonly IUserAccessor _userAccessor;

        public RoleController(IRepository<Role> roleRepository, IMediator mediator, IUserAccessor userAccessor)
        {
            _roleRepository = roleRepository;
            _mediator = mediator;
            _userAccessor = userAccessor;
        }

        [Route("GetAll")]
        [HttpPost]
        [AccessControl(PermissionsSD.QueryRole)]
        public async Task<ListActionResult<RoleSummary>> GetAll([FromBody] GridQuery query, CancellationToken cancellationToken)
        {
            return
                await _roleRepository.GetAllAsync<RoleSummary>(
                    query,
                    cancellationToken);
        }

        [Route("SelectList")]
        [HttpGet]
        public async Task<List<SelectSummary>> SelectList(CancellationToken cancellationToken)
        {
            return
                await _roleRepository.GetAllAsync<SelectSummary>();
        }

        [Route("Find")]
        [HttpGet]
        [AccessControl(PermissionsSD.QueryRole)]
        public async Task<RoleDetails> Find([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            return await _roleRepository.FirstOrDefaultAsync<RoleDetails>(b => b.Id == id, cancellationToken);
        }

        [Route("Create")]
        [HttpPost]
        [AccessControl(PermissionsSD.CommandRole)]
        public async Task<CommandResponse> Create([FromBody] CreateRoleCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }

        [Route("Update")]
        [HttpPut]
        [AccessControl(PermissionsSD.CommandRole)]
        public async Task<CommandResponse> Update([FromBody] UpdateRoleCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }

        [Route("Remove")]
        [HttpDelete]
        [AccessControl(PermissionsSD.CommandRole)]
        public async Task<CommandResponse> Remove([FromQuery] DeleteRoleCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }
    }
}
