using Domain;
using Domain.Dtos.Shared;
using Domain.Entities.Account;
using MediatR;

namespace Infrastructure.CQRS.Account.Companies
{
    public class AddUserToCompanyCommand : IRequest<CommandResponse>
    {
        public Guid RoleId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }


    public class AddUserToCompanyCommandHandler : IRequestHandler<AddUserToCompanyCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public AddUserToCompanyCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(AddUserToCompanyCommand request, CancellationToken cancellationToken)
        {
            if (!_context.Users.Any(b => b.Id == request.UserId))
                return CommandResponse.Failure(400, "کاربر انتخاب شده در سیستم وجود ندارد");

            if (!_context.Roles.Any(b => b.Id == request.RoleId))
                return CommandResponse.Failure(400, "نقش انتخاب شده در سیستم وجود ندارد");

            if (!_context.Company.Any(b => b.Id == request.CompanyId))
                return CommandResponse.Failure(400, "سازمان انتخاب شده در سیستم وجود ندارد");

            if (_context.Act.Any(b => b.UserId == request.UserId && b.CompanyId == request.CompanyId))
                return CommandResponse.Failure(400, "کاربر انتخاب شده در سازمان فوق وجود دارد");

            if (request.RoleId == SD.AgentId && _context.Act.Any(b => b.CompanyId == request.CompanyId && b.RoleId == SD.AgentId))
                return CommandResponse.Failure(400, "نمیتوان برای یک سازمان دو نماینده انتخاب کرد");

            var act = new Act
            {
                CompanyId = request.CompanyId,
                RoleId = request.RoleId,
                UserId = request.UserId,
                Id = Guid.NewGuid()
            };

            _context.Act.Add(act);

            if (await _context.SaveChangesAsync() > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(500, "مشکل داخلی سرور");
        }
    }
}
