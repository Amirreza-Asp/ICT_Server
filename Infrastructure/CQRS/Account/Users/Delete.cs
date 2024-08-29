using Application.Services.Interfaces;
using Domain.Dtos.Shared;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.CQRS.Account.Users
{
    public class DeleteUserCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
    }

    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserAccessor _userAccessor;

        public DeleteUserCommandHandler(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, IUserAccessor userAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(b => b.Id == request.Id && b.IsActive);

            if (user == null)
                return CommandResponse.Success();

            var companyId = _userAccessor.GetCompanyId();

            //if ((bool)(user.Companies.Where(b => b.Id == companyId).FirstOrDefault()?.IsAdmin))
            //    return CommandResponse.Failure(400, "نماینده سازمان را نمیتوان حذف کرد");

            user.IsActive = false;
            _context.Users.Update(user);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(500, "مشکل داخلی سرور");
        }
    }
}
