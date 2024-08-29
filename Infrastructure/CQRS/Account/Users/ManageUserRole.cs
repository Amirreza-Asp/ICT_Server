using Application.Services.Interfaces;
using Domain;
using Domain.Dtos.Shared;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.CQRS.Account.Users
{
    public class ManageUserRoleCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
        public Guid RoleId { get; set; }
    }


    public class ManageUserRoleCommandHandler : IRequestHandler<ManageUserRoleCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserAccessor _userAccessor;

        public ManageUserRoleCommandHandler(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, IUserAccessor userAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(ManageUserRoleCommand request, CancellationToken cancellationToken)
        {
            var companyId = _userAccessor.GetCompanyId();

            if (_context.Act.Any(b => b.CompanyId == companyId.Value && b.RoleId == SD.AgentId))
                return CommandResponse.Failure(400, "یک سازمان نمیتواند بیشتر از یک نماینده داشته باشد");


            var user =
            await _context.Users
                .Include(b => b.Token)
                .FirstOrDefaultAsync(b => b.IsActive && request.Id == b.Id);

            if (user == null)
                return CommandResponse.Failure(400, "کاربر انتخاب شده در سیستم وجود ندارد");

            if (!_context.Roles.Any(b => b.Id == request.RoleId))
                return CommandResponse.Failure(400, "نقش انتخاب شده در سیستم وجود ندارد");

            //user.RoleId = request.RoleId;
            _context.Users.Update(user);

            if (user.Token != null)
            {
                user.Token.IsActive = false;
                _context.Tokens.Update(user.Token);
            }


            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(500, "مشکل داخلی سرور");
        }
    }
}
