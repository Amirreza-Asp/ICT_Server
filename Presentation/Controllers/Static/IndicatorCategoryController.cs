using Application.Repositories;
using Domain;
using Domain.Dtos.Shared;
using Domain.Dtos.Static;
using Infrastructure.CQRS.Static.IndicatorCategories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.CustomeAttributes;

namespace Presentation.Controllers.Static
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class IndicatorCategoryController : ControllerBase
    {
        private readonly IIndicatorCategoryRepository _incRepo;
        private readonly IMediator _mediator;

        public IndicatorCategoryController(IIndicatorCategoryRepository incRepo, IMediator mediator)
        {
            _incRepo = incRepo;
            _mediator = mediator;
        }

        [HttpGet]
        [Route("GetNested")]
        public async Task<List<NestedIndicators>> GetNested(CancellationToken cancellationToken)
        {
            return await _incRepo.GetNestedIndicatorsAsync(cancellationToken);
        }

        [HttpGet]
        [Route("Find")]
        public async Task<IndicatorCategoryDetails> Find([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            return await _incRepo.FirstOrDefaultAsync<IndicatorCategoryDetails>(b => b.Id == id, cancellationToken);
        }

        [HttpPost]
        [Route("Create")]
        [AccessControl(PermissionsSD.CommandIndicatorCategory)]
        public async Task<CommandResponse> Creare([FromBody] CreateIndicatorCategoryCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }

        [HttpPut]
        [Route("Update")]
        [AccessControl(PermissionsSD.CommandIndicatorCategory)]
        public async Task<CommandResponse> Update([FromBody] UpdateIndicatorCategoryCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }

        [HttpDelete]
        [Route("Delete")]
        [AccessControl(PermissionsSD.CommandIndicatorCategory)]
        public async Task<CommandResponse> Delete([FromQuery] DeleteIndicatorCategoryCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }

    }
}
