using Domain.Dtos.Shared;
using Domain.Entities.Account;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CQRS.Account.Companies
{
    public class CreateCompanyCommand : IRequest<CommandResponse>
    {
        [Required]
        public Guid ParentId { get; set; }
        [Required]
        public String Title { get; set; }

        public String Province { get; set; }
        public String City { get; set; }
    }

    public class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public CreateCompanyCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
        {
            if (!await _context.Company.AnyAsync(b => b.Id == request.ParentId))
                return CommandResponse.Failure(400, "سازمان بالا دست در سیستم وجود ندارد");

            var company = new Company
            {
                Id = Guid.NewGuid(),
                ParentId = request.ParentId,
                Title = request.Title,
                Province = request.Province,
                City = request.City
            };

            _context.Company.Add(company);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success(company.Id);

            return CommandResponse.Failure(500, "مشکل داخلی سرور");
        }
    }
}
