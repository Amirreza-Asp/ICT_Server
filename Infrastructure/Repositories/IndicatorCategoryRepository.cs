using Application.Repositories;
using AutoMapper;
using Domain.Dtos.Static;
using Domain.Entities.Static;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class IndicatorCategoryRepository : Repository<IndicatorCategory>, IIndicatorCategoryRepository
    {
        public IndicatorCategoryRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public async Task<List<NestedIndicators>> GetNestedIndicatorsAsync(CancellationToken cancellationToken)
        {
            var data =
                await _context.IndicatorCategories
                    .ToListAsync(cancellationToken);

            var parents = data.Where(b => b.ParentId == null).ToList();
            var nestedParents = new List<NestedIndicators>();

            foreach (var parent in parents)
                nestedParents.Add(ConvertToNested(parent, data));

            return nestedParents;
        }

        private NestedIndicators ConvertToNested(IndicatorCategory parent, List<IndicatorCategory> items)
        {
            var nestedParent = new NestedIndicators
            {
                Id = parent.Id,
                Title = parent.Title,
            };

            foreach (var child in items)
            {
                if (parent.Id == child.ParentId)
                {
                    nestedParent.Childs.Add(ConvertToNested(child, items));
                }
            }

            return nestedParent;
        }
    }
}
