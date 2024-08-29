using Application.Repositories;
using Application.Services.Interfaces;
using Domain;
using Domain.Dtos.Shared;
using Domain.Entities.Business;
using Domain.Queries.Shared;
using Infrastructure.CQRS.Business.HardwareEquipments;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.CustomeAttributes;

namespace Presentation.Controllers.Business
{
    [Route("api/[controller]")]
    [ApiController]
    public class HardwareEquipmentController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IRepository<HardwareEquipment> _repo;
        private readonly IUserAccessor _userAccessor;

        public HardwareEquipmentController(IMediator mediator, IRepository<HardwareEquipment> repo, IUserAccessor userAccessor)
        {
            _mediator = mediator;
            _repo = repo;
            _userAccessor = userAccessor;
        }

        [Route("GetAll")]
        [HttpPost]
        [AccessControl(PermissionsSD.QueryHardwareEquipment)]
        public async Task<ListActionResult<HardwareEquipment>> GetAll([FromBody] GridQuery query, CancellationToken cancellationToken)
        {
            var companyId = _userAccessor.GetCompanyId();
            return await _repo.GetAllAsync<HardwareEquipment>(query, b => b.CompanyId == companyId.Value, cancellationToken);
        }

        [HttpGet("Find/{id}")]
        [AccessControl(PermissionsSD.QueryHardwareEquipment)]
        public async Task<HardwareEquipment> Find(Guid id, CancellationToken cancellation)
        {
            return await _repo.FirstOrDefaultAsync<HardwareEquipment>(d => d.Id == id, cancellationToken: cancellation);
        }

        [Route("Create")]
        [HttpPost]
        [AccessControl(PermissionsSD.CommandHardwareEquipment)]
        public async Task<CommandResponse> Create([FromBody] CreateHardwareEquipmentCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }

        [Route("Update")]
        [HttpPut]
        [AccessControl(PermissionsSD.CommandBigGoal)]
        public async Task<CommandResponse> Update([FromBody] UpdateHardwareEquipmentCommand command, CancellationToken cancellation)
        {
            return await _mediator.Send(command, cancellation);
        }

        [Route("Delete")]
        [HttpDelete]
        [AccessControl(PermissionsSD.CommandBigGoal)]
        public async Task<CommandResponse> Delete([FromQuery] DeleteHardwareEquipmentCommand command, CancellationToken cancellation)
        {
            return await _mediator.Send(command, cancellation);
        }
    }
}
