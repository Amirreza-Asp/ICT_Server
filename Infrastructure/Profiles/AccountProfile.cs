using Application.Services.Interfaces;
using AutoMapper;
using Domain;
using Domain.Dtos.Account.Acts;
using Domain.Dtos.Account.Permissions;
using Domain.Dtos.Account.Roles;
using Domain.Dtos.Account.SSO;
using Domain.Dtos.Account.User;
using Domain.Dtos.Account.Users;
using Domain.Dtos.Companies;
using Domain.Dtos.Shared;
using Domain.Entities.Account;
using System.Data;

namespace Infrastructure.Profiles
{
    public class AccountProfile : Profile
    {


        public AccountProfile()
        {
            CreateMap<SSOUserInfo, UserProfile>()
                .ForMember(b => b.NationalId, d => d.MapFrom(e => e.nationalId))
                .ForMember(b => b.FullName, d => d.MapFrom(e => String.Concat(e.firstName, " ", e.lastName)))
                .ForMember(b => b.Mobile, d => d.MapFrom(e => e.mobile))
                .ForMember(b => b.Gender, d => d.MapFrom(e => e.gender));

            var rnd = new Random();

            CreateMap<Act, UserSummary>()
                .ForMember(b => b.FullName, d => d.MapFrom(e => String.Concat(e.User.Name, ' ', e.User.Family)))
                .ForMember(b => b.Role, d => d.MapFrom(e => e.Role.Title))
                .ForMember(b => b.CreatedAt, d => d.MapFrom(e => e.User.CreatedAt))
                .ForMember(b => b.NationalId, d => d.MapFrom(e => e.User.NationalId));
            CreateMap<User, UserListDto>()
                .ForMember(b => b.FullName, d => d.MapFrom(e => e.Name + ' ' + e.Family))
                .ForMember(b => b.NationalId, d => d.MapFrom(e => e.NationalId))
                .ForMember(b => b.PhoneNumber, d => d.MapFrom(e => "0" + rnd.NextInt64(1000000000, 9999999999)));
            CreateMap<User, SelectSummary>()
                .ForMember(b => b.Id, d => d.MapFrom(e => e.Id))
                .ForMember(b => b.Title, d => d.MapFrom(e => String.Concat(e.Name, ' ', e.Family)));


            CreateMap<PermissionItem, PermissionSummary>()
                .ForMember(b => b.Value, d => d.MapFrom(e => e.PageValue));

            CreateMap<Role, RoleSummary>();
            CreateMap<Role, SelectSummary>()
                .ForMember(b => b.Id, e => e.MapFrom(s => s.Id))
                .ForMember(b => b.Title, e => e.MapFrom(s => s.Title));
            CreateMap<Role, RoleDetails>()
                .ForMember(b => b.Permissios, d => d.MapFrom(e => e.Permissions.Select(b => b.PermissionId)));


            CreateMap<UserJoinRequest, UserRequestsSummary>()
                .ForMember(b => b.RequestTime, d => d.MapFrom(e => e.CreatedAt));

            CreateMap<CompsRequestData, Company>()
                .ForMember(b => b.Id, d => d.MapFrom(e => e.id))
                .ForMember(b => b.Title, d => d.MapFrom(e => e.nameUniversity))
                .ForMember(b => b.Logo, d => d.MapFrom(e => e.logoUniversity))
                .ForMember(b => b.PortalUrl, d => d.MapFrom(e => e.portalUrl))
                .ForMember(b => b.NationalSerial, d => d.MapFrom(e => e.nationalSerial))
                .ForMember(b => b.Latitude, d => d.MapFrom(e => e.latitude))
                .ForMember(b => b.Longitude, d => d.MapFrom(e => e.longitude))
                .ForMember(b => b.City, d => d.MapFrom(e => e.cityName))
                .ForMember(b => b.Province, d => d.MapFrom(e => e.provinceName))
                .ForMember(b => b.UniversityType, d => d.MapFrom(e => e.universityType))
                .ForMember(b => b.Status, d => d.MapFrom(e => e.status))
                .ForMember(b => b.CreateAt, d => d.MapFrom(e => e.createAt));

            CreateMap<Company, NestedCompanies>()
               .ForMember(b => b.IndicatorCount, d => d.MapFrom(e => e.Indicators.Count))
               .ForMember(b => b.UsersCount, d => d.MapFrom(e => e.Acts.DistinctBy(b => b.UserId).Count()))
               .ForMember(b => b.Childs, d => d.Ignore())
               .ForMember(b => b.Agent, d => d.MapFrom(e =>
                    e.Acts.Any(b => b.RoleId == SD.AgentId) ?
                    e.Acts.First(b => b.RoleId == SD.AgentId).User.Name + " " + e.Acts.First(b => b.RoleId == SD.AgentId).User.Family : null));


            // Acts
            CreateMap<Act, ActSummary>()
                .ForMember(e => e.RoleTitle, d => d.MapFrom(b => b.Role.Title))
                .ForMember(e => e.CompanyTitle, d => d.MapFrom(b => b.Company.Title));

        }

        private List<String> Names =>
            new List<string>
            {
                "امیر",
                "محمد",
                "زهرا",
                "پریسا",
                "پیمان",
                "رضا",
            };

        private List<String> Families =>
            new List<string>
            {
                "قادری",
                "محمدی",
                "مرادی",
                "حیدری",
                "منوچهری",
                "نویدی",
            };
    }
}
