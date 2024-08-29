using Domain.Dtos.Static;
using Domain.Entities.Static;

namespace Application.Repositories
{
    public interface IIndicatorCategoryRepository : IRepository<IndicatorCategory>
    {
        Task<List<NestedIndicators>> GetNestedIndicatorsAsync(CancellationToken cancellationToken);
    }
}
