using Application.Services.Interfaces;
using AutoMapper;
using Domain.Dtos.Shared;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CQRS.Business.Systems
{
    public class CreateSystemCommand : IRequest<CommandResponse>
    {

        [Required]
        public String Title { get; set; }

        public String Description { get; set; }

        [Required]
        public String ProgrammingLanguage { get; set; }

        [Required]
        public String Database { get; set; }

        [Required]
        public String Framework { get; set; }

        [Required]
        public String Development { get; set; }

        [Required]
        public String Company { get; set; }

        [Required]
        public String OS { get; set; }

        [Required]
        public String SupportType { get; set; }


    }

    public class CreateSystemCommandHandler : IRequestHandler<CreateSystemCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserAccessor _userAccessor;

        public CreateSystemCommandHandler(ApplicationDbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor, IUserAccessor userAccessor)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(CreateSystemCommand request, CancellationToken cancellationToken)
        {
            var system = _mapper.Map<Domain.Entities.Business.System>(request);


            var comapnyId = _userAccessor.GetCompanyId();
            system.CompanyId = comapnyId.Value;

            _context.Add(system);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success(system.Id);

            return CommandResponse.Failure(500);
        }
    }
}
