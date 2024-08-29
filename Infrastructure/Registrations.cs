using Application.Repositories;
using Application.Services.Interfaces;
using Infrastructure.Initializer;
using Infrastructure.Repositories;
using Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Infrastructure
{
    public static class Registrations
    {

        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Sql context
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                //options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                //options.UseInMemoryDatabase("ITC");
            });

            // Initializer
            services.AddScoped<IDbInitializer, DbInitializer>();

            // Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ISSOService, SSOService>();
            services.AddScoped<ITokenValidate, TokenValidate>();
            services.AddScoped<IUserAccessor, UserAccessor>();

            // Repositories
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<IIndicatorCategoryRepository, IndicatorCategoryRepository>();
            services.AddScoped<IBigGoalRepository, BigGoalRepository>();

            // MediatR
            services.AddMediatR(Assembly.GetExecutingAssembly());

            // AutoMapper
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            return services;
        }


    }
}
