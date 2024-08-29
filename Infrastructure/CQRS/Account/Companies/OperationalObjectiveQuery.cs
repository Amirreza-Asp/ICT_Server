using AutoMapper;
using Domain.Dtos.Companies;
using Domain.Dtos.OperationalObjectives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.CQRS.Account.Companies
{
    public class OperationalObjectiveQuery : IRequest<List<CompanyOperationalObjectives>>
    {
        public List<Guid> Companies { get; set; }
        public String BigGoal { get; set; }
        public int ProgressFrom { get; set; } = 0;
        public int ProgressTo { get; set; } = 100;
    }

    public class OperationalObjectiveQueryHandler : IRequestHandler<OperationalObjectiveQuery, List<CompanyOperationalObjectives>>
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public OperationalObjectiveQueryHandler(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<CompanyOperationalObjectives>> Handle(OperationalObjectiveQuery request, CancellationToken cancellationToken)
        {
            var data =
                await _context.OperationalObjectives
                    .Include(b => b.Indicators)
                        .ThenInclude(b => b.Indicator)
                    .Where(b =>
                            request.Companies.Contains(b.BigGoal.Programs.First().Program.CompanyId) &&
                            (String.IsNullOrWhiteSpace(request.BigGoal) || b.BigGoal.Title.Contains(request.BigGoal.Trim())))
                    .Select(op => new CompanyOperationalObjectives
                    {
                        CompanyId = op.BigGoal.Programs.First().Program.CompanyId,
                        CompanyName = op.BigGoal.Programs.First().Program.Company.Title,
                        OperationalObjecives = new List<OperationalObjectiveListDto>
                        {
                            new OperationalObjectiveListDto
                            {
                                Id = op.Id,
                                BigGoal = op.BigGoal.Title,
                                Progress = op.Progress,
                                Title = op.Title
                            }
                        }
                    })
                    .ToListAsync(cancellationToken);

            var coo = new List<CompanyOperationalObjectives>();
            data.ForEach(item =>
            {
                int progress = item.OperationalObjecives.First().Progress;

                if (progress >= request.ProgressFrom && progress <= request.ProgressTo &&
                    coo.Any(b => b.CompanyId == item.CompanyId))
                    coo.Find(b => b.CompanyId == item.CompanyId).OperationalObjecives.Add(item.OperationalObjecives.First());
                else if (progress >= request.ProgressFrom && progress <= request.ProgressTo)
                    coo.Add(item);
            });

            return coo;
        }
    }
}
