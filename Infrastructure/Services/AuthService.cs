using Application.Services.Interfaces;
using Application.Utility;
using AutoMapper;
using Domain;
using Domain.Dtos.Account;
using Domain.Dtos.Account.Acts;
using Domain.Dtos.Account.Cookies;
using Domain.Dtos.Account.Permissions;
using Domain.Dtos.Account.SSO;
using Domain.Dtos.Shared;
using Domain.Entities.Account;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _memoryCache;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMapper _mapper;
        private readonly IUserAccessor _userAccessor;

        public AuthService(ApplicationDbContext context, IMemoryCache cacheContext, IHttpContextAccessor contextAccessor, IMapper mapper, IUserAccessor userAccessor)
        {
            _context = context;
            _memoryCache = cacheContext;
            _contextAccessor = contextAccessor;
            _mapper = mapper;
            _userAccessor = userAccessor;
        }

        public async Task<bool> CheckPermission(String permissionFlag)
        {
            var roleId = _userAccessor.RoleId();
            String permissionCacheKey = $"permissions-{roleId}";
            var permissionFlags = new List<string>();

            if (!_memoryCache.TryGetValue(permissionCacheKey, out permissionFlags))
            {
                permissionFlags = await GetFromContext(roleId);

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(1));

                _memoryCache.Set(permissionCacheKey, permissionFlags, cacheEntryOptions);
            }

            return permissionFlag != null && permissionFlags.Contains(permissionFlag);
        }

        public async Task<String> GetRoleAsync(string nationalCode)
        {
            var companyId = _userAccessor.GetCompanyId();

            return
                await _context.Act
                    .Where(b => b.User.NationalId == nationalCode && b.CompanyId == companyId)
                    .Select(b => b.Role.Title)
                    .FirstOrDefaultAsync();
        }

        public async Task<bool> ExistsAsync(String nationalId)
        {
            return await _context.Users.AnyAsync(b => b.NationalId == nationalId);
        }

        public async Task CreateAsync(ProfileRequest user)
        {
            var userRole = new User
            {
                Id = Guid.NewGuid(),
                NationalId = user.data.nationalId,
                IsActive = true
            };

            _context.Users.Add(userRole);
            await _context.SaveChangesAsync();
        }

        public async Task LoginAsync(String nationalId, String uswToken)
        {
            if (!await _context.Users.AnyAsync(b => b.NationalId == nationalId))
                await AddUserAsync(nationalId);

            var userId =
                await _context.Users
                    .AsNoTracking()
                    .Where(b => b.NationalId == nationalId)
                    .Select(b => b.Id)
                    .FirstOrDefaultAsync();

            var data = new UserTempDataCookies { UserId = ProtectorData.Encrypt(userId.ToString()), UswToken = ProtectorData.Encrypt(uswToken) };

            _contextAccessor.HttpContext.Response.Cookies.Append("user-temp-info", JsonSerializer.Serialize(data), new CookieOptions()
            {
                Expires = DateTime.Now.AddMonths(1),
                HttpOnly = true,
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None
            });
        }


        public async Task<CommandResponse> ChooseActAsync(ChooseActDto model)
        {
            var jsonUserTempData = _contextAccessor.HttpContext.Request.Cookies["user-temp-info"];

            if (jsonUserTempData == null)
                return CommandResponse.Failure(400, "اطلاعات شما پیدا نشد ، لطفا دوباره وارد شوید");

            var userTempData = JsonSerializer.Deserialize<UserTempDataCookies>(jsonUserTempData);

            var act =
                await _context.Act
                    .Include(b => b.User)
                        .ThenInclude(b => b.Token)
                    .Include(b => b.User)
                        .ThenInclude(b => b.RefreshToken)
                    .FirstOrDefaultAsync(b =>
                        b.Id == model.ActId);

            if (act == null || act.User == null)
                return CommandResponse.Failure(400, "اطلاعات شما پیدا نشد ، لطفا دوباره وارد شوید");

            var ip = _contextAccessor.HttpContext.Request.HttpContext.Connection.RemoteIpAddress.ToString();

            var token = JWTokenService.GenerateToken(act.User.NationalId, act.RoleId.ToString(), ip, act.CompanyId);
            var hashedToken = ProtectorData.Encrypt(token);

            UpsertToken(ip, hashedToken, act.User);
            var rfValue = UpsertRefreshToken(act.User);

            if (await _context.SaveChangesAsync() > 0)
            {
                SetCookie(hashedToken, rfValue, ProtectorData.Decrypt(userTempData.UswToken));
                return CommandResponse.Success();
            }

            return CommandResponse.Failure(400, "مشکل در احراز هویت کاربر");
        }


        public async Task<bool> LoginWithRefreshTokenAsync(Guid refreshToken)
        {
            var uswToken = _contextAccessor.HttpContext.Request.Cookies[SD.UswToken];
            var userToken = _contextAccessor.HttpContext.Request.Cookies[SD.AuthInfo];

            return false;

            if (uswToken == null)
                return false;

            var rfToken =
                await _context.RefreshTokens
                    .Where(b => b.Value == refreshToken)
                    .Include(b => b.User)
                        .ThenInclude(b => b.Token)
                    .Include(b => b.User)
                        .ThenInclude(b => b.Act)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

            if (rfToken == null || rfToken.User == null || rfToken.IsActive == false || rfToken.Expiration < DateTime.Now || rfToken.User.IsActive == false)
                return false;


            //_memoryCache.Set("User", new Data { CompanyId = rfToken.User.CompanyId, NationalId = rfToken.User.NationalId }, DateTimeOffset.Now.AddDays(1));

            var ip = _contextAccessor.HttpContext.Request.HttpContext.Connection.RemoteIpAddress.ToString();

            // rfToken.User.Role.Title
            //var newToken = JWTokenService.GenerateToken(rfToken.User.NationalId, "", ip, rfToken.User.Companies.First().Id);
            //var tokenHash = ProtectorData.Encrypt(newToken);
            //UpsertToken(ip, tokenHash, rfToken.User);

            await _context.SaveChangesAsync();

            //SetCookie(tokenHash, refreshToken, ProtectorData.Decrypt(uswToken));

            return true;
        }

        public async Task<CommandResponse> LogoutAsync()
        {
            var nationalId = _userAccessor.GetNationalId();
            var token =
                await _context.Tokens
                    .Where(b => b.User.NationalId == nationalId)
                    .FirstOrDefaultAsync();

            if (token == null)
                return CommandResponse.Failure(400, "کاربر به سیستم وارد نشده است");

            token.IsActive = false;
            _context.Tokens.Update(token);
            _memoryCache.Remove($"user-{nationalId}");

            //_memoryCache.Remove("User");


            _contextAccessor.HttpContext.Response.Cookies.Delete(SD.AuthInfo, new CookieOptions
            {
                Secure = true,
                HttpOnly = true,
                IsEssential = true,
                SameSite = SameSiteMode.None
            });
            _contextAccessor.HttpContext.Response.Cookies.Delete(SD.UswToken, new CookieOptions
            {
                Secure = true,
                HttpOnly = true,
                IsEssential = true,
                SameSite = SameSiteMode.None
            });


            if (await _context.SaveChangesAsync() > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(500, "مشکل داخلی سرور");
        }

        public async Task<List<PermissionSummary>> GetPermissionsAsync(string nationalCode)
        {
            var permissons =
                await _context.Permissions
                    .Where(b => b.Roles.Any(b => b.Role.Acts.Any(b => b.User.NationalId == nationalCode)))
                    .ToListAsync();

            List<PermissionSummary> data = new List<PermissionSummary>();
            permissons.ForEach(item =>
            {
                data.Add(new PermissionSummary
                {
                    Title = item.Title,
                    Value = item.Discriminator != nameof(PermissionItem) ? null : ((PermissionItem)item).PageValue
                });
            });

            return data;
        }


        #region Utilities
        private void SetCookie(String hashedToken, Guid refreshToken, String uswToken)
        {
            var authInfo = new AuthInfo
            {
                Token = hashedToken,
                RefreshToken = ProtectorData.Encrypt(refreshToken.ToString()),
            };

            _contextAccessor.HttpContext.Response.Cookies.Append(SD.AuthInfo, JsonSerializer.Serialize(authInfo), new CookieOptions()
            {
                Expires = DateTime.Now.AddMonths(1),
                HttpOnly = true,
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None
            });

            _contextAccessor.HttpContext.Response.Cookies.Append(SD.UswToken, ProtectorData.Encrypt(uswToken), new CookieOptions()
            {
                Expires = DateTime.Now.AddMonths(1),
                HttpOnly = true,
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None
            });
        }

        private async Task AddUserAsync(String nationalId)
        {
            var userRole = new User
            {
                Id = Guid.NewGuid(),
                IsActive = true,
                NationalId = nationalId,
                //RoleId = SD.AdminRoleId
            };

            _context.Users.Add(userRole);
            await _context.SaveChangesAsync();
        }

        private void UpsertToken(String ip, String tokenHash, User user)
        {
            if (user.Token == null)
            {
                var token = new Token
                {
                    Id = Guid.NewGuid(),
                    Expiration = DateTime.Now.AddMinutes(SD.ExpirationTime),
                    Ip = ip,
                    HashValue = tokenHash,
                    UserId = user.Id,
                    IsActive = true
                };

                _context.Tokens.Add(token);
            }
            else
            {
                _memoryCache.Remove($"user-{user.NationalId}");
                user.Token.HashValue = tokenHash;
                user.Token.Expiration = DateTime.Now.AddMinutes(SD.ExpirationTime);
                user.Token.Ip = ip;
                user.Token.IsActive = true;

                _context.Tokens.Update(user.Token);
            }
        }

        private Guid UpsertRefreshToken(User user)
        {
            if (user.RefreshToken == null)
            {
                var refreshToken = new RefreshToken
                {
                    Id = Guid.NewGuid(),
                    Expiration = DateTime.Now.AddMonths(1),
                    UserId = user.Id,
                    Value = Guid.NewGuid(),
                };

                _context.RefreshTokens.Add(refreshToken);
                return refreshToken.Value;
            }
            else
            {
                user.RefreshToken.Value = Guid.NewGuid();
                user.RefreshToken.Expiration = DateTime.Now.AddMonths(1);

                _context.RefreshTokens.Update(user.RefreshToken);
                return user.RefreshToken.Value;
            }
        }

        private async Task<List<String>> GetFromContext(Guid roleId)
        {
            var permissions =
                await _context.RolePermissions
                    .Where(b => b.RoleId == roleId && b.Permission.Discriminator == nameof(PermissionItem))
                    .Select(b => (PermissionItem)b.Permission)
                    .ToListAsync();

            return permissions.Select(b => b.PageValue).ToList();
        }

        #endregion

    }




}
