using Application.Repositories;
using Domain;
using Domain.Dtos.Shared;
using Domain.Dtos.Transitions;
using Domain.Entities.Business;
using Domain.Queries.Shared;
using Domain.Utiltiy;
using Infrastructure.CQRS.Business.Transitions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Presentation.CustomeAttributes;

namespace Presentation.Controllers.Business
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransitionController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IRepository<Transition> _transitionprtRepo;
        private readonly IRepository<TransitionIndicator> _transitionIndicatorRepo;

        public TransitionController(IMediator mediator, IRepository<Transition> transitionprtRepo, IRepository<TransitionIndicator> projectIndicatorRepo)
        {
            _mediator = mediator;
            _transitionprtRepo = transitionprtRepo;
            _transitionIndicatorRepo = projectIndicatorRepo;
        }

        [HttpPost]
        [Route("GetAll")]
        [AccessControl(PermissionsSD.QueryTransition)]
        public async Task<List<TransitionSummary>> GetAll([FromBody] GridQuery query, CancellationToken cancellationToken)
        {
            var strParentId = query.Filters.Find(b => b.column.ToLower() == "parentid").value;
            Guid? parentId = String.IsNullOrEmpty(strParentId) ? null : Guid.Parse(strParentId);
            query.Filters = query.Filters.Where(b => b.column.ToLower() != "parentid").ToList();

            return
                await _transitionprtRepo
                    .GetAllAsync<TransitionSummary>(
                        query: query,
                        filters: b => b.ParentId == parentId,
                        include: source => source
                           .Include(b => b.Indicators)
                               .ThenInclude(b => b.Indicator),
                        cancellationToken
                    );

        }

        [HttpGet("Find/{id}")]
        [AccessControl(PermissionsSD.QueryTransition)]
        public async Task<TransitionDetails> Find(Guid id, CancellationToken cancellationToken)
        {
            var data = await _transitionprtRepo.FirstOrDefaultAsync<TransitionDetails>(b => b.Id == id, cancellationToken: cancellationToken);
            if (data == null)
                return null;

            data.Indicators.ForEach(item =>
            {
                item.ScheduleProgress = Calculator.CalcProgress(item);
                item.ScheduleCurrentValue = Calculator.CalcCurrentValue(item);
            });

            return data;
        }

        [Route("Create")]
        [HttpPost]
        [AccessControl(PermissionsSD.CommandTransition)]
        public async Task<CommandResponse> Create([FromBody] CreateTransitionCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }

        [Route("Update")]
        [HttpPut]
        [AccessControl(PermissionsSD.CommandTransition)]
        public async Task<CommandResponse> Update([FromBody] UpdateTransitionCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }

        [Route("Delete")]
        [HttpDelete]
        [AccessControl(PermissionsSD.CommandTransition)]
        public async Task<CommandResponse> Remove([FromQuery] DeleteTransitionCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }


        [Route("AddIndicator")]
        [HttpPost]
        [AccessControl(PermissionsSD.CommandTransition)]
        public async Task<CommandResponse> AddIndicator([FromBody] AddTransitionIndicatorCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }

        [Route("GetIndicators")]
        [HttpGet]
        public async Task<List<IndicatorDetails>> GetIndicators([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            return await _transitionIndicatorRepo.GetAllAsync<IndicatorDetails>(b => b.TransitionId == id, cancellationToken: cancellationToken);
        }

        [Route("RemoveIndicator")]
        [HttpDelete]
        [AccessControl(PermissionsSD.CommandTransition)]
        public async Task<CommandResponse> RemoveIndicator([FromQuery] RemoveTransitionIndicatorCommand command, CancellationToken cancellation)
        {
            return await _mediator.Send(command, cancellation);
        }
    }
}
