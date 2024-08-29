using AutoMapper;
using Domain.Dtos.BigGoals;
using Domain.Dtos.Companies;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.CQRS.Account.Companies
{
    public class CompanyBigGoalsQuery : IRequest<List<CompanyBigGoals>>
    {
        public List<Guid> Companies { get; set; }
        public String Year { get; set; }
        public int ProgressFrom { get; set; } = 0;
        public int ProgressTo { get; set; } = 100;
    }

    public class CompanyBigGoalsQueryHandler : IRequestHandler<CompanyBigGoalsQuery, List<CompanyBigGoals>>
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CompanyBigGoalsQueryHandler(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<CompanyBigGoals>> Handle(CompanyBigGoalsQuery request, CancellationToken cancellationToken)
        {
            var data =
             await _context.BigGoals
                 .Include(b => b.Indicators)
                    .ThenInclude(b => b.Indicator)
                 .Where(b =>
                         request.Companies.Contains(b.Programs.Select(b => b.Program.CompanyId).First()) &&
                         (String.IsNullOrWhiteSpace(request.Year) || b.Programs.Any(b => b.Program.StartedAt.Year.ToString() == request.Year.Trim())))
                 .Select(op => new CompanyBigGoals
                 {
                     CompanyId = op.Programs.First().Program.CompanyId,
                     CompanyName = op.Programs.First().Program.Company.Title,
                     BigGoals = new List<BigGoalsListDto>
                     {
                            new BigGoalsListDto
                            {
                                Id = op.Id,
                                Year = op.Programs.First().Program.StartedAt.Year.ToString(),
                                Title = op.Title,
                                Progress = op.Progress
                            }
                     }
                 })
                 .ToListAsync(cancellationToken);

            var coo = new List<CompanyBigGoals>();
            data.ForEach(item =>
            {
                int progress = item.BigGoals.First().Progress;

                if (progress >= request.ProgressFrom && progress <= request.ProgressTo && coo.Any(b => b.CompanyId == item.CompanyId))
                    coo.Find(b => b.CompanyId == item.CompanyId).BigGoals.Add(item.BigGoals.First());
                else if (progress >= request.ProgressFrom && progress <= request.ProgressTo)
                    coo.Add(item);
            });

            return coo;
        }
    }
}
