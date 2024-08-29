using Application.Repositories;
using Domain.Dtos.Account.Permissions;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.Account
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionRepository _permissionRepo;

        public PermissionController(IPermissionRepository permissionRepo)
        {
            _permissionRepo = permissionRepo;
        }

        [Route("NestedPermissions")]
        [HttpGet]
        public async Task<NestedPermissions> NestedPermissions(CancellationToken cancellationToken)
        {
            return await _permissionRepo.GetNestedPermissionsAsync(cancellationToken);
        }

        [Route("Tasks")]
        [HttpGet]
        public async Task<PermissionTask> Tasks(CancellationToken cancellationToken)
        {
            return await _permissionRepo.Task(cancellationToken);
        }

    }
}
