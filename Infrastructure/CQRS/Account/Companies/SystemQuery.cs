using Domain.Dtos.Companies;
using Domain.Dtos.Refrences;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.CQRS.Account.Companies
{
    public class CompaniesSystemQuery : IRequest<List<CompanySystem>>
    {
        public List<Guid> Companies { get; set; }
        public String Company { get; set; } = "";
        public String Database { get; set; } = "";
    }

    public class CompaniesSystemQueryHandler : IRequestHandler<CompaniesSystemQuery, List<CompanySystem>>
    {
        private readonly ApplicationDbContext _context;

        public CompaniesSystemQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CompanySystem>> Handle(CompaniesSystemQuery request, CancellationToken cancellationToken)
        {
            var data =
               await _context.Systems
                   .Where(b =>
                           request.Companies.Contains(b.CompanyId) &&
                           (String.IsNullOrWhiteSpace(request.Company) || b.BuildInCompany.Contains(request.Company.Trim())) &&
                           (String.IsNullOrWhiteSpace(request.Database) || b.Database.Contains(request.Database.Trim())))
                   .Select(op => new CompanySystem
                   {
                       CompanyId = op.CompanyId,
                       CompanyName = op.Company.Title,
                       Systems = new List<SystemDto>
                       {
                            new SystemDto
                            {
                                Id = op.Id,
                                Company = op.BuildInCompany,
                                Database = op.Database,
                                Title = op.Title
                            }
                       }
                   })
                   .ToListAsync(cancellationToken);

            var coo = new List<CompanySystem>();
            data.ForEach(item =>
            {
                if (coo.Any(b => b.CompanyId == item.CompanyId))
                    coo.Find(b => b.CompanyId == item.CompanyId).Systems.Add(item.Systems.First());
                else
                    coo.Add(item);
            });

            return coo;
        }
    }
}
