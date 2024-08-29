using Application.Services.Interfaces;
using Application.Utility;
using Domain;
using Domain.Dtos.Account;
using Domain.Entities.Account;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
using System.Text.Json;

namespace Infrastructure.Services
{

    public class TokenValidate : ITokenValidate
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthService _authService;
        private readonly IMemoryCache _memoryCache;

        public TokenValidate(ApplicationDbContext context, IAuthService authService, IMemoryCache memoryCache)
        {
            _context = context;
            _authService = authService;
            _memoryCache = memoryCache;
        }

        public async Task Execute(TokenValidatedContext context)
        {
            var authInfoJson = context.Request.Cookies[SD.AuthInfo];
            var uswToken = context.Request.Cookies[SD.UswToken];

            if (String.IsNullOrEmpty(authInfoJson) || String.IsNullOrEmpty(uswToken))
            {
                context.Fail("احراز هویت نامعتبر می باشد");
                return;
            }

            var authInfo = JsonSerializer.Deserialize<AuthInfo>(authInfoJson);
            var token = ProtectorData.Decrypt(authInfo.Token);
            var refreshToken = Guid.Parse(ProtectorData.Decrypt(authInfo.RefreshToken));

            var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
            if (claimsIdentity?.Claims == null || !claimsIdentity.Claims.Any())
            {
                context.Fail("احراز هویت نامعتبر می باشد");
                return;
            }

            var userNationalId = claimsIdentity.GetUserNationalId();

            String userCacheKey = $"user-{userNationalId}";
            var user = new User();

            if (!_memoryCache.TryGetValue(userCacheKey, out user))
            {
                user = await _context.Users
                    .Include(b => b.Act)
                        .ThenInclude(b => b.Role)
                    .Include(x => x.Token)
                    .Include(b => b.RefreshToken)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.NationalId == userNationalId);

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(1));

                _memoryCache.Set(userCacheKey, user, cacheEntryOptions);
            }

            if (user == null || user.IsActive == false)
            {
                context.Fail("احراز هویت نامعتبر می باشد");
                return;
            }

            if (user.Token == null || user.Token.IsActive == false)
            {
                context.Fail("احراز هویت نامعتبر می باشد");
                return;
            }

            if (user.Token.Expiration <= DateTime.UtcNow)
            {
                if (!await _authService.LoginWithRefreshTokenAsync(refreshToken))
                {
                    context.Fail("احراز هویت نامعتبر می باشد");
                    return;
                }
            }

            if (user.Token.Ip != context.Request.HttpContext.Connection.RemoteIpAddress.ToString())
            {
                context.Fail("ip شما نسبت به زمان احراز هویت تغییر کرده است و نشست شما نامعتبر می باشد");
                return;
            }

            var role = claimsIdentity.GetRole();
            if (string.IsNullOrEmpty(role))
            {
                context.Fail("احراز هویت نامعتبر می باشد");
                return;
            }

            if (!user.Act.Any(b => b.Role.Id.ToString() == role))
            {
                context.Fail("احراز هویت نامعتبر می باشد");
                return;
            }
        }
    }
}
