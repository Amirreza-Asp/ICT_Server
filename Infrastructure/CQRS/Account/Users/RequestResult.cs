using Domain.Dtos.Shared;
using Domain.Entities.Account;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.CQRS.Account.Users
{
    public class UserRequestResultCommand : IRequest<CommandResponse>
    {
        public bool Accept { get; set; }
        public Guid RequstId { get; set; }
        public Guid RoleId { get; set; }
    }

    public class UserRequestResultCommandHandler : IRequestHandler<UserRequestResultCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public UserRequestResultCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(UserRequestResultCommand request, CancellationToken cancellationToken)
        {
            var joinRequest = await _context.UsersJoinRequests.FirstOrDefaultAsync(b => b.Id == request.RequstId);

            if (!request.Accept)
            {
                if (joinRequest == null)
                    return CommandResponse.Success();

                _context.Remove(joinRequest);
                if (await _context.SaveChangesAsync(cancellationToken) > 0)
                    return CommandResponse.Success();
            }
            else
            {
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    NationalId = joinRequest.NationalId
                };

                var userCompany = new Act
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    CompanyId = joinRequest.CompanyId.Value,
                    RoleId = request.RoleId
                };

                _context.Users.Add(user);
                _context.Act.Add(userCompany);
                _context.UsersJoinRequests.Remove(joinRequest);

                if (await _context.SaveChangesAsync(cancellationToken) > 0)
                    return CommandResponse.Success();
            }

            return CommandResponse.Failure(400, "مشکل داخلی سرور");
        }
    }
}
