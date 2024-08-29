using Application.Services.Interfaces;
using Domain.Dtos.Shared;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.CQRS.Account.Roles
{
    public class DeleteRoleCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
    }

    public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserAccessor _userAccessor;

        public DeleteRoleCommandHandler(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, IUserAccessor userAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            var role =
                await _context.Roles
                    .AsNoTracking()
                    .Where(b =>
                        b.Id == request.Id)
                    .FirstOrDefaultAsync(cancellationToken);

            if (role == null)
                return CommandResponse.Success();

            if (_context.Act.Any(b => b.RoleId == request.Id))
                return CommandResponse.Failure(400, "این نقش در سیستم برای برخی کاربران استفاده شده و نمیتوان ان را حذف کرد");

            _context.Roles.Remove(role);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(500, "مشکل داخلی سرور");
        }
    }
}
