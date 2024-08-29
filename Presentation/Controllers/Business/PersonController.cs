using Application.Repositories;
using Application.Services.Interfaces;
using Domain;
using Domain.Dtos.People;
using Domain.Dtos.Shared;
using Domain.Entities.Business;
using Domain.Queries.Shared;
using Infrastructure.CQRS.Business.People;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.CustomeAttributes;

namespace Presentation.Controllers.Business
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IRepository<Person> _repo;
        private readonly IUserAccessor _userAccessor;

        public PersonController(IMediator mediator, IRepository<Person> repo, IUserAccessor userAccessor)
        {
            _mediator = mediator;
            _repo = repo;
            _userAccessor = userAccessor;
        }


        [Route("GetAll")]
        [HttpPost]
        [AccessControl(PermissionsSD.QueryPerson)]
        public async Task<ListActionResult<PersonSummary>> GetAll([FromBody] GridQuery query, CancellationToken cancellationToken)
        {
            var companyId = _userAccessor.GetCompanyId();
            return await _repo.GetAllAsync<PersonSummary>(query, b => b.CompanyId == companyId.Value, cancellationToken);
        }

        [HttpGet("Find/{id}")]
        [AccessControl(PermissionsSD.QueryPerson)]
        public async Task<PersonSummary> Find(Guid id, CancellationToken cancellationToken)
        {
            return await _repo.FirstOrDefaultAsync<PersonSummary>(b => b.Id == id, cancellationToken: cancellationToken);
        }

        [Route("Create")]
        [HttpPost]
        [AccessControl(PermissionsSD.CommandPerson)]
        public async Task<CommandResponse> Create([FromBody] CreatePersonCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }

        [Route("Update")]
        [HttpPut]
        [AccessControl(PermissionsSD.CommandPerson)]
        public async Task<CommandResponse> Update([FromBody] UpdatePersonCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);


        [Route("Delete")]
        [HttpDelete]
        [AccessControl(PermissionsSD.CommandPerson)]
        public async Task<CommandResponse> Delete([FromQuery] DeletePersonCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);
    }
}
