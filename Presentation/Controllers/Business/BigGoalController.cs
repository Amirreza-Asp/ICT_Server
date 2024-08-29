using Application.Repositories;
using Application.Services.Interfaces;
using Domain;
using Domain.Dtos.BigGoals;
using Domain.Dtos.Refrences;
using Domain.Dtos.Shared;
using Domain.Queries.Shared;
using Domain.Utiltiy;
using Infrastructure.CQRS.Business.BigGoals;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.CustomeAttributes;

namespace Presentation.Controllers.Business
{
    [Route("api/[controller]")]
    [ApiController]
    public class BigGoalController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IBigGoalRepository _repo;
        private readonly IUserAccessor _userAccessor;

        public BigGoalController(IMediator mediator, IBigGoalRepository repo, IUserAccessor userAccessor)
        {
            _mediator = mediator;
            _repo = repo;
            _userAccessor = userAccessor;
        }


        [Route("DropDown")]
        [HttpPost]
        public async Task<ListActionResult<BigGoalSelectList>> DropDown(GridQuery query, CancellationToken cancellationToken)
        {
            var companyId = _userAccessor.GetCompanyId();
            return await _repo.GetAllAsync<BigGoalSelectList>(query, b => b.Programs.Any(u => u.Program.IsActive) && b.Programs.Select(b => b.Program).Any(s => s.CompanyId == companyId.Value), cancellationToken);
        }

        [Route("GetAll")]
        [HttpPost]
        [AccessControl(PermissionsSD.QueryBigGoal)]
        public async Task<ListActionResult<BigGoalSummary>> GetAll(GridQuery query, CancellationToken cancellationToken)
        {
            return await _repo.GetSummaryAsync(query, cancellationToken);
        }

        [Route("GetRefrences")]
        [HttpGet]
        [AccessControl(PermissionsSD.QueryBigGoal)]
        public async Task<Refrences> GetRefrences([FromQuery] Guid bigGoalId, CancellationToken cancellationToken) =>
            await _repo.GetRefrencesAsync(bigGoalId, cancellationToken);


        [HttpGet("Find/{id}")]
        [AccessControl(PermissionsSD.QueryBigGoal)]
        public async Task<BigGoalDetails> Find(Guid id, CancellationToken cancellationToken)
        {
            var data = await _repo.FirstOrDefaultAsync<BigGoalDetails>(b => b.Id == id, cancellationToken: cancellationToken);
            data.Progress = Calculator.CalcProgress(data.Indicators);

            data.Indicators.ToList().ForEach(item =>
            {
                item.ScheduleProgress = Calculator.CalcProgress(item);
                item.ScheduleCurrentValue = Calculator.CalcCurrentValue(item);
            });

            return data;
        }

        [Route("Update")]
        [HttpPut]
        [AccessControl(PermissionsSD.CommandBigGoal)]
        public async Task<CommandResponse> Update([FromBody] UpdateBigGoalCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }

        [Route("Create")]
        [HttpPost]
        [AccessControl(PermissionsSD.CommandBigGoal)]
        public async Task<CommandResponse> Create([FromBody] CreateBigGoalCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);

        [Route("Remove")]
        [HttpDelete]
        [AccessControl(PermissionsSD.CommandBigGoal)]
        public async Task<CommandResponse> Remove([FromQuery] DeleteBigGoalCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }


        [Route("AddIndicator")]
        [HttpPost]
        [AccessControl(PermissionsSD.CommandBigGoal)]
        public async Task<CommandResponse> AddIndicator([FromBody] AddBigGoalIndicatorCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }


        [Route("RemoveIndicator")]
        [HttpDelete]
        [AccessControl(PermissionsSD.CommandBigGoal)]
        public async Task<CommandResponse> RemoveIndicator([FromQuery] RemoveBigGoalIndicatorCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }
    }
}
