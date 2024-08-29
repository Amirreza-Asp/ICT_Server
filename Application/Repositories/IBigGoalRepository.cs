using Domain.Dtos.BigGoals;
using Domain.Dtos.Refrences;
using Domain.Dtos.Shared;
using Domain.Entities.Business;
using Domain.Queries.Shared;

namespace Application.Repositories
{
    public interface IBigGoalRepository : IRepository<BigGoal>
    {
        public Task<ListActionResult<BigGoalSummary>> GetSummaryAsync(GridQuery query, CancellationToken cancellationToken);
        Task<Refrences> GetRefrencesAsync(Guid bigGoalId, CancellationToken cancellationToken);

    }
}
