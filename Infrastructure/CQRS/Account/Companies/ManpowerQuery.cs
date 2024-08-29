using Domain.Dtos.Companies;
using Domain.Dtos.People;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.CQRS.Account.Companies
{
    public class CompaniesManpowerQuery : IRequest<List<CompanyManpower>>
    {
        public List<Guid> Companies { get; set; }
        public String JobTitle { get; set; }
        public String Education { get; set; }
    }

    public class CompaniesManpowerQueryHanlder : IRequestHandler<CompaniesManpowerQuery, List<CompanyManpower>>
    {
        private readonly ApplicationDbContext _context;

        public CompaniesManpowerQueryHanlder(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CompanyManpower>> Handle(CompaniesManpowerQuery request, CancellationToken cancellationToken)
        {
            var data =
               await _context.People
                   .Where(b =>
                           request.Companies.Contains(b.CompanyId) &&
                   (String.IsNullOrWhiteSpace(request.JobTitle) || b.JobTitle.Contains(request.JobTitle.Trim())) &&
                   (String.IsNullOrWhiteSpace(request.Education) || b.Education.Contains(request.Education.Trim())))
                   .Select(op => new CompanyManpower
                   {
                       CompanyId = op.CompanyId,
                       CompanyName = op.Company.Title,
                       Manpowers = new List<ManpowerDto>
                       {
                            new ManpowerDto
                            {
                                Id = op.Id,
                                JobTitle = op.JobTitle,
                                FullName = String.Concat(op.Name , ' ' , op.Family),
                                Education = op.Education
                            }
                       }
                   })
                   .ToListAsync(cancellationToken);

            var coo = new List<CompanyManpower>();
            data.ForEach(item =>
            {
                if (coo.Any(b => b.CompanyId == item.CompanyId))
                    coo.Find(b => b.CompanyId == item.CompanyId).Manpowers.Add(item.Manpowers.First());
                else
                    coo.Add(item);
            });

            return coo;
        }
    }
}
