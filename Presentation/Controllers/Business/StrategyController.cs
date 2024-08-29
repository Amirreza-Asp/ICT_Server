using Application.Repositories;
using Application.Services.Interfaces;
using Domain;
using Domain.Dtos.Shared;
using Domain.Dtos.Strategies;
using Domain.Entities.Business;
using Domain.Queries.Shared;
using Infrastructure.CQRS.Business.Strategies;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.CustomeAttributes;

namespace Presentation.Controllers.Business
{
    [Route("api/[controller]")]
    [ApiController]
    public class StrategyController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IRepository<Strategy> _strategyRepo;
        private readonly IUserAccessor _userAccessor;

        public StrategyController(IMediator mediator, IRepository<Strategy> strategyRepo, IUserAccessor userAccessor)
        {
            _mediator = mediator;
            _strategyRepo = strategyRepo;
            _userAccessor = userAccessor;
        }


        [Route("GetAll")]
        [HttpPost]
        [AccessControl(PermissionsSD.QueryStrategy)]
        public async Task<List<StrategySummary>> GetAll([FromBody] GridQuery query, CancellationToken cancellationToken)
        {
            var companyId = _userAccessor.GetCompanyId().Value;
            return
                await _strategyRepo.GetAllAsync<StrategySummary>(
                       query: query,
                       filters: b => b.Program.IsActive && b.Program.CompanyId == companyId,
                       include: null,
                       cancellationToken);
        }

        [HttpPost]
        [Route("Create")]
        [AccessControl(PermissionsSD.CommandStrategy)]
        public async Task<CommandResponse> Create([FromBody] CreateStrategyCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);

        [HttpPut]
        [Route("Update")]
        [AccessControl(PermissionsSD.CommandStrategy)]
        public async Task<CommandResponse> Update([FromBody] UpdateStrategyCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);

        [HttpDelete]
        [Route("Delete")]
        [AccessControl(PermissionsSD.CommandStrategy)]
        public async Task<CommandResponse> Delete([FromQuery] DeleteStrategyCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);
    }
}
