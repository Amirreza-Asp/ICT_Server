using Application.Services.Interfaces;
using Domain.Dtos.Shared;
using Domain.Entities.Account;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CQRS.Account.Roles
{
    public class CreateRoleCommand : IRequest<CommandResponse>
    {
        [Required(ErrorMessage = "عنوان را وارد کنید")]
        public String Title { get; set; }

        [Required(ErrorMessage = "توضیحات را وارد کنید")]
        public String Description { get; set; }


        public List<Guid> Permissions { get; set; }
    }

    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserAccessor _userAccessor;

        public CreateRoleCommandHandler(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, IUserAccessor userAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            if (_context.Roles.Any(b => b.Title == request.Title))
                return CommandResponse.Failure(400, $"نقش با عنوان {request.Title} در سیستم وجود دارد");

            var roleId = Guid.NewGuid();
            var companyId = _userAccessor.GetCompanyId();

            var role = new Role
            {
                Id = roleId,
                Title = request.Title,
                Description = request.Description,
                Permissions = request.Permissions.Select(permissionId => new RolePermission
                {
                    Id = Guid.NewGuid(),
                    PermissionId = permissionId,
                    RoleId = roleId
                }).ToList()
            };

            _context.Roles.Add(role);

            if (await _context.SaveChangesAsync() > 0)
                return CommandResponse.Success(roleId);


            return CommandResponse.Failure(500, "خطای سرور");
        }
    }
}
