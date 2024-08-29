using Application.Services.Interfaces;
using Domain.Dtos.Shared;
using Domain.Entities.Account;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CQRS.Account.Roles
{

    public class UpdateRoleCommand : IRequest<CommandResponse>
    {
        [Required]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "عنوان را وارد کنید")]
        public String Title { get; set; }

        [Required(ErrorMessage = "توضیحات را وارد کنید")]
        public String Description { get; set; }


        public List<Guid> Permissions { get; set; }
    }

    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _memoryCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserAccessor _userAccessor;

        public UpdateRoleCommandHandler(ApplicationDbContext context, IMemoryCache memoryCache, IHttpContextAccessor httpContextAccessor, IUserAccessor userAccessor)
        {
            _context = context;
            _memoryCache = memoryCache;
            _httpContextAccessor = httpContextAccessor;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            var role =
                await _context.Roles
                    .Where(b =>
                        b.Id == request.Id)
                    .Include(b => b.Permissions)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(cancellationToken);

            if (role == null)
                CommandResponse.Failure(400, "نقش انتخاب شده در سیستم وجود ندارد");

            foreach (var permission in role.Permissions)
                _context.RolePermissions.Remove(permission);


            foreach (var permissionId in request.Permissions)
            {
                var permission = new RolePermission
                {
                    Id = Guid.NewGuid(),
                    PermissionId = permissionId,
                    RoleId = role.Id
                };

                _context.RolePermissions.Add(permission);
            }

            role.Title = request.Title;
            role.Description = request.Description;

            _context.Roles.Update(role);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                var usersNationalIds =
                    await _context.Act
                        .Where(b => b.RoleId == role.Id)
                        .Select(b => b.User.NationalId)
                        .AsNoTracking()
                        .ToListAsync(cancellationToken);

                foreach (var nationalId in usersNationalIds)
                    _memoryCache.Remove($"permissions-{nationalId}");

                return CommandResponse.Success();
            }

            return CommandResponse.Failure(500, "مشکل سرور");
        }
    }
}
