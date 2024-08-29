using Domain.Dtos.Account.Acts;
using Domain.Dtos.Account.Permissions;
using Domain.Dtos.Account.SSO;
using Domain.Dtos.Shared;

namespace Application.Services.Interfaces
{
    public interface IAuthService
    {
        Task<bool> CheckPermission(string permissionFlag);
        Task<List<PermissionSummary>> GetPermissionsAsync(String nationalCode);

        Task<String> GetRoleAsync(String nationalCode);
        Task LoginAsync(String nationalId, String uswToken);
        Task<CommandResponse> ChooseActAsync(ChooseActDto model);
        Task<bool> LoginWithRefreshTokenAsync(Guid refreshToken);
        Task<CommandResponse> LogoutAsync();

        Task<bool> ExistsAsync(String nationalId);
        Task CreateAsync(ProfileRequest user);
    }
}
