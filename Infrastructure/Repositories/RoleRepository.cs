using Application.Repositories;
using AutoMapper;
using Domain.Entities.Account;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Repositories
{
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        private readonly IMemoryCache _memoryCache;
        public RoleRepository(ApplicationDbContext context, IMapper mapper, IMemoryCache memoryCache) : base(context, mapper)
        {
            _memoryCache = memoryCache;
        }

        public override void Update(Role role)
        {
            var usersNationalIds =
                _context.Act
                    .Where(b => b.RoleId == role.Id)
                    .Select(b => b.User.NationalId)
                    .ToList();

            foreach (var nationalId in usersNationalIds)
                _memoryCache.Remove($"permissions-{nationalId}");

            base.Update(role);
        }
    }
}
