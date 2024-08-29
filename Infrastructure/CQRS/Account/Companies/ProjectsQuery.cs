using Domain.Dtos.Companies;
using Domain.Dtos.Transitions;
using Domain.Utiltiy;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.CQRS.Account.Companies
{
    public class CompanyProjectsQuery : IRequest<List<CompanyProjects>>
    {
        public List<Guid> Companies { get; set; }
        public String BigGoal { get; set; }
    }

    public class CompanyProjectsQueryHandler : IRequestHandler<CompanyProjectsQuery, List<CompanyProjects>>
    {
        private readonly ApplicationDbContext _context;

        public CompanyProjectsQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CompanyProjects>> Handle(CompanyProjectsQuery request, CancellationToken cancellationToken)
        {
            var data =
               await _context.Transitions
               .Where(b =>
                           request.Companies.Contains(b.OperationalObjective.BigGoal.Programs.First().Program.CompanyId) &&
                           (String.IsNullOrWhiteSpace(request.BigGoal) || b.OperationalObjective.BigGoal.Title.Contains(request.BigGoal.Trim())))
               .Select(b => new CompanyProjects
               {
                   CompanyId = b.OperationalObjective.BigGoal.Programs.First().Program.CompanyId,
                   CompanyName = b.OperationalObjective.BigGoal.Programs.First().Program.Company.Title,
                   Projects = new List<TransitionListDto> {
                       new TransitionListDto {
                           BigGoal = b.OperationalObjective.BigGoal.Title,
                           Id = b.Id,
                           Title = b.Title
                       }
                   }
               })
              .ToListAsync(cancellationToken);

            var indicators =
                await _context.TransitionIndicators
                    .Where(b => data.Select(b => b.Projects[0].Id).Contains(b.TransitionId))
                    .Include(b => b.Indicator)
                    .ToListAsync(cancellationToken);

            data.ForEach(item =>
            {
                var itemIndicators =
                    indicators.Where(b => b.TransitionId == item.Projects[0].Id)
                    .Select(b => b.Indicator)
                    .ToList();

                item.Projects[0].Progress = Calculator.CalcProgress(itemIndicators);
            });

            var coo = new List<CompanyProjects>();
            data.ForEach(item =>
            {
                if (coo.Any(b => b.CompanyId == item.CompanyId))
                    coo.Find(b => b.CompanyId == item.CompanyId).Projects.Add(item.Projects.First());
                else
                    coo.Add(item);
            });

            return coo;
        }
    }
}
