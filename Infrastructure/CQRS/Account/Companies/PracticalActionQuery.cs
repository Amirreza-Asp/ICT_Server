using Domain.Dtos.Companies;
using Domain.Dtos.Transitions;
using Domain.Entities.Business;
using Domain.Utiltiy;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.CQRS.Account.Companies
{
    public class CompanyPracticalActionQuery : IRequest<List<CompanyTransitions>>
    {
        public List<Guid> Companies { get; set; }
        public String BigGoal { get; set; }
    }

    public class CompanyPracticalActionQueryHandler : IRequestHandler<CompanyPracticalActionQuery, List<CompanyTransitions>>
    {
        private readonly ApplicationDbContext _context;

        public CompanyPracticalActionQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CompanyTransitions>> Handle(CompanyPracticalActionQuery request, CancellationToken cancellationToken)
        {
            var data =
               await _context.Transitions
                   .Where(b =>
                           request.Companies.Contains(b.OperationalObjective.BigGoal.Programs.First().Program.CompanyId) &&
                           b.Type == TransitionType.Action &&
                           (String.IsNullOrWhiteSpace(request.BigGoal) || b.OperationalObjective.BigGoal.Title.Contains(request.BigGoal.Trim())))
                   .Select(op => new CompanyTransitions
                   {
                       CompanyId = op.OperationalObjective.BigGoal.Programs.First().Program.CompanyId,
                       CompanyName = op.OperationalObjective.BigGoal.Programs.First().Program.Company.Title,
                       PracticalActions = new List<TransitionListDto>
                       {
                            new TransitionListDto
                            {
                                Id = op.Id,
                                BigGoal = op.OperationalObjective.BigGoal.Title,
                                Title = op.Title
                            }
                       }
                   })
                   .ToListAsync(cancellationToken);

            var indicators =
               await _context.TransitionIndicators
                   .Where(b => data.Select(b => b.PracticalActions[0].Id).Contains(b.TransitionId))
                   .Include(b => b.Indicator)
                   .ToListAsync(cancellationToken);

            data.ForEach(item =>
            {
                var itemIndicators =
                    indicators.Where(b => b.TransitionId == item.PracticalActions[0].Id)
                    .Select(b => b.Indicator)
                    .ToList();

                item.PracticalActions[0].Progress = Calculator.CalcProgress(itemIndicators);
            });

            var coo = new List<CompanyTransitions>();
            data.ForEach(item =>
            {
                if (coo.Any(b => b.CompanyId == item.CompanyId))
                    coo.Find(b => b.CompanyId == item.CompanyId).PracticalActions.Add(item.PracticalActions.First());
                else
                    coo.Add(item);
            });

            return coo;
        }
    }
}
