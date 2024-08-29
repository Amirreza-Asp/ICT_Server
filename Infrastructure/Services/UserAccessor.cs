using Application.Services.Interfaces;
using Domain;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Infrastructure.Services
{
    public class UserAccessor : IUserAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? GetCompanyId()
        {
            return Guid.Parse("aa12b4d2-652c-407a-a569-9edcd1e2c467");
            var claims = (_httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity);
            var companyId = claims.FindFirst(AppClaims.CompanyId).Value;
            if (Guid.Empty == Guid.Parse(companyId))
                return null;
            return Guid.Parse(companyId);
        }

        public string GetNationalId()
        {
            return "3360408330";
            var claims = (_httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity);
            return claims.FindFirst(AppClaims.NationalId).Value;
        }

        public Guid RoleId()
        {
            return SD.AgentId;
            var claims = (_httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity);
            var roleId = claims.FindFirst(AppClaims.Role).Value;

            if (String.IsNullOrEmpty(roleId))
                return Guid.Empty;

            try
            {
                return Guid.Parse(roleId);
            }
            catch (Exception ex)
            {
                return Guid.Empty;
            }
        }
    }
}
