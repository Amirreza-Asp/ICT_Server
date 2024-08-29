using Application.Repositories;
using Application.Services.Interfaces;
using Domain;
using Domain.Dtos.Account.Permissions;
using Domain.Entities.Account;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUserAccessor _userAccessor;

        public PermissionRepository(ApplicationDbContext context, IHttpContextAccessor contextAccessor, IUserAccessor userAccessor)
        {
            _context = context;
            _contextAccessor = contextAccessor;
            _userAccessor = userAccessor;
        }

        public async Task<NestedPermissions> GetNestedPermissionsAsync(CancellationToken cancellationToken)
        {
            var companyId = _userAccessor.GetCompanyId();

            var permissions = await _context.Permissions
                .Where(b =>
                    b.Discriminator == nameof(PermissionContainer))
                .Include(b => ((PermissionContainer)b).Childrens)
                .ToListAsync(cancellationToken);

            var containerList = permissions.Select(b => (PermissionContainer)b).ToList();
            var parent = containerList.First(b => b.Id == SD.TopPermissionId);

            return ConvertToSelectListPermissions(containerList, parent);
        }

        public async Task<PermissionTask> Task(CancellationToken cancellationToken)
        {
            var roleId = _userAccessor.RoleId();
            var permissions =
                await _context.Permissions
                    .Where(b => b.Roles.Any(u => u.RoleId == roleId) && b.Discriminator == nameof(PermissionContainer))
                    .Include(b => ((PermissionContainer)b).Childrens)
                    .ToListAsync(cancellationToken);

            var containers = permissions.Select(b => ((PermissionContainer)b)).ToList();
            var parent = containers.First(b => b.Id == SD.TopPermissionId);
            return ConvertToSelectListTask(containers, parent);
        }

        private NestedPermissions ConvertToSelectListPermissions(List<PermissionContainer> permissionContainers, PermissionContainer permission)
        {
            var selectListPermission = new NestedPermissions(permission.Id, permission.Title);

            foreach (var pe in permission.Childrens)
            {
                if (pe.Discriminator == nameof(PermissionContainer))
                {
                    var convertedChild =
                        ConvertToSelectListPermissions(
                            permissionContainers,
                            permissionContainers.First(b => b.Id == pe.Id));
                    selectListPermission.Childs.Add(convertedChild);
                }
                else
                    selectListPermission.Childs.Add(new NestedPermissions(pe.Id, pe.Title));
            }

            return selectListPermission;
        }

        private PermissionTask ConvertToSelectListTask(List<PermissionContainer> permissionContainers, PermissionContainer permission)
        {
            var selectListPermission = new PermissionTask(permission.Title);

            foreach (var pe in permission.Childrens)
            {
                if (pe.Discriminator == nameof(PermissionContainer))
                {
                    var convertedChild =
                        ConvertToSelectListTask(
                            permissionContainers,
                            permissionContainers.First(b => b.Id == pe.Id));
                    selectListPermission.Childs.Add(convertedChild);
                }
                else
                    selectListPermission.Childs.Add(new PermissionTask(pe.Title));
            }

            return selectListPermission;
        }
    }
}
