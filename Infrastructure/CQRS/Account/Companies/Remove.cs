using Domain.Dtos.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.CQRS.Account.Companies
{
    public class RemoveCompanyCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
    }

    public class RemoveCompanyCommandHandler : IRequestHandler<RemoveCompanyCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public RemoveCompanyCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(RemoveCompanyCommand request, CancellationToken cancellationToken)
        {
            if (_context.Company.Where(b => b.Id == request.Id).Any(b => b.Childs.Any()))
                return CommandResponse.Failure(400, "برای حذف این سازمان ابتدا زیر سازمان های ان را حذف کنید");

            var company = await _context.Company.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (company == null)
                return CommandResponse.Success();

            _context.Company.Remove(company);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(500, "مشکل داخلی سرور");
        }
    }
}
