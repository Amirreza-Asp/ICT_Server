using Application.Repositories;
using Application.Services.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Dtos.BigGoals;
using Domain.Dtos.People;
using Domain.Dtos.Refrences;
using Domain.Dtos.Shared;
using Domain.Entities.Business;
using Domain.Queries.Shared;
using Infrastructure.Utility;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class BigGoalRepository : Repository<BigGoal>, IBigGoalRepository
    {
        private readonly IUserAccessor _userAccessor;


        public BigGoalRepository(ApplicationDbContext context, IMapper mapper, IUserAccessor userAccessor) : base(context, mapper)
        {
            _userAccessor = userAccessor;
        }

        public async Task<Refrences> GetRefrencesAsync(Guid bigGoalId, CancellationToken cancellationToken)
        {
            var financials =
                await _context.Transitions
                    .Where(b => b.OperationalObjective.BigGoalId == bigGoalId)
                    .AsNoTracking()
                    .Select(b => b.Financials)
                    .ToListAsync(cancellationToken);

            var refrencesTitle = new List<String>();
            foreach (var item in financials)
            {
                refrencesTitle.AddRange(item.Distinct().Select(b => b.Title));
            }
            refrencesTitle = refrencesTitle.Distinct().ToList();

            var companyId = _userAccessor.GetCompanyId().Value;

            var personsId = await _context.People
                    .Where(b => refrencesTitle.Contains(b.Name + " " + b.Family))
                    .Select(b => b.Id)
                    .ToListAsync(cancellationToken);

            var persons =
                await _context.People
                    .Where(b => personsId.Distinct().Contains(b.Id) && b.CompanyId == companyId)
                    .AsNoTracking()
                    .ProjectTo<PersonSummary>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

            var hardwares =
                await _context.HardwareEquipment
                    .Where(b => refrencesTitle.Contains(b.Title) && b.CompanyId == companyId)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

            var systems =
                await _context.Systems
                   .Where(b => refrencesTitle.Contains(b.Title) && b.CompanyId == companyId)
                   .AsNoTracking()
                   .ProjectTo<SystemDetails>(_mapper.ConfigurationProvider)
                   .ToListAsync(cancellationToken);

            return new Refrences
            {
                Systems = systems,
                HardwareEquipments = hardwares,
                Persons = persons
            };
        }

        public async Task<ListActionResult<BigGoalSummary>> GetSummaryAsync(GridQuery query, CancellationToken cancellationToken)
        {
            var actionResult = new ListActionResult<BigGoalSummary>();

            var queryContext = _context.BigGoals.AsQueryable();
            var companyId = _userAccessor.GetCompanyId().Value;


            queryContext =
                queryContext.Where(b =>
                        b.Programs.Any(u => u.Program.IsActive) &&
                        b.Programs.Select(b => b.Program)
                            .Any(s => s.CompanyId == companyId));

            //filter
            if (query.Filters != null && query.Filters.Any())
            {
                // var filterExpression = QueryUtility.FilterExpression<T>(args.Filtered[0].column, args.Filtered[0].value);
                for (int i = 0; i < query.Filters.Count; i++)
                {
                    var filterExpression = QueryUtility.FilterExpression<BigGoal>(query.Filters[i].column, query.Filters[i].value);
                    if (filterExpression != null)
                        queryContext = queryContext.Where(filterExpression);
                }
            }

            //total count
            var total = await queryContext.CountAsync(cancellationToken);

            //sort
            if (query.Sorted != null && query.Sorted.Length > 0)
            {
                for (int i = 0; i < query.Sorted.Length; i++)
                {
                    queryContext = queryContext.SortMeDynamically(query.Sorted[i].column, query.Sorted[i].desc);
                }
            }

            //data
            var result = await queryContext
                .Include(b => b.Indicators)
                    .ThenInclude(b => b.Indicator)
                .Skip((query.Page - 1) * query.Size)
                .Take(query.Size)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            actionResult.Data = _mapper.Map<List<BigGoalSummary>>(result);
            actionResult.Total = total;
            actionResult.Page = query.Page;
            actionResult.Size = query.Size;

            return actionResult;
        }
    }
}
