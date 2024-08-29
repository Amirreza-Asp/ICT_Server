using Microsoft.AspNetCore.Http;

namespace Domain
{
    public static class SD
    {
        public const String ClientId = "Test593a12712eba48e6b7d5b9431fa71aa0";
        public const String ClientSecret = "Testaef6ccca445e4170b1c8de10f0788e84";
        public const String DefaultNationalId = "1234567890";

        public const String AuthInfo = "auth_info";
        public const String UswToken = "usw-token";

        public static Guid TopPermissionId = Guid.Parse("08151D71-9D6C-43EF-85A6-16043DCB6B3A");

        public static Guid AgentId = Guid.Parse("64BEFD80-D431-47CD-828C-C791EFD5A1CF");
        public static Guid ManagmentPermissionId = Guid.Parse("8B078358-17DA-4DD0-9BE4-31125D5F7914");

        public const int ExpirationTime = 15;

        public static String GetRedirectUrl(IHttpContextAccessor _accessor)
        {
            return _accessor.HttpContext.Request.Scheme + "://" + _accessor.HttpContext.Request.Host.Value + "/api/account/authorizeLogin";
        }
    }

}
