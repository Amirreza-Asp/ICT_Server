using Application.Repositories;
using Application.Services.Interfaces;
using Domain;
using Domain.Dtos.Account.Users;
using Domain.Dtos.Shared;
using Domain.Entities.Account;
using Domain.Queries.Shared;
using Infrastructure.CQRS.Account.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.CustomeAttributes;

namespace Presentation.Controllers.Account
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IRepository<UserJoinRequest> _userJoinRequestsRepo;
        private readonly IUserAccessor _userAccessor;
        private readonly IRepository<Act> _actRepository;

        public UserController(IMediator mediator, IRepository<UserJoinRequest> userJoinRequestsRepo, IUserAccessor userAccessor, IRepository<Act> actRepository)
        {
            _mediator = mediator;
            _userJoinRequestsRepo = userJoinRequestsRepo;
            _userAccessor = userAccessor;
            _actRepository = actRepository;
        }

        [HttpPost]
        [Route("GetAll")]
        [AccessControl(PermissionsSD.UsersList)]
        public async Task<ListActionResult<UserSummary>> GetAll([FromBody] GridQuery query, CancellationToken cancellationToken)
        {
            return
                await _actRepository
                    .GetAllAsync<UserSummary>(query, b => b.CompanyId == _userAccessor.GetCompanyId() && b.User.IsActive, cancellationToken);
        }


        [Route("Delete")]
        [HttpDelete]
        [AccessControl(PermissionsSD.RemoveUser)]
        public async Task<CommandResponse> Remove([FromQuery] DeleteUserCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }

        [Route("ManageRole")]
        [HttpPut]
        [AccessControl(PermissionsSD.ManageUserRole)]
        public async Task<CommandResponse> ManageUserRole([FromBody] ManageUserRoleCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }

        [Route("JoinRequests")]
        [HttpPost]
        [AccessControl(PermissionsSD.UsersRequests)]
        public async Task<ListActionResult<UserRequestsSummary>> GetJoinRequests([FromBody] GridQuery query, CancellationToken cancellationToken)
        {
            var companyId = _userAccessor.GetCompanyId();
            return await _userJoinRequestsRepo.GetAllAsync<UserRequestsSummary>(query, b => b.CompanyId == companyId, cancellationToken);
        }

        [HttpPost]
        [Route("RequestResult")]
        [AccessControl(PermissionsSD.UsersRequests)]
        public async Task<CommandResponse> RequestResult([FromBody] UserRequestResultCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }
    }
}
