using Application.Services.Interfaces;
using AutoMapper;
using Domain.Dtos.Shared;
using Domain.Entities.Business;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CQRS.Business.HardwareEquipments
{
    public class CreateHardwareEquipmentCommand : IRequest<CommandResponse>
    {
        [Required]
        public String Title { get; set; }

        [Required]
        public String BrandName { get; set; }

        [Range(1, int.MaxValue)]
        public int Count { get; set; }

        public String Description { get; set; }

        [Required]
        public String SupportType { get; set; }

        [Required]
        public Guid CompanyId { get; set; }
    }

    public class CreateHardwareEquipmentCommandHandler : IRequestHandler<CreateHardwareEquipmentCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserAccessor _userAccessor;

        public CreateHardwareEquipmentCommandHandler(ApplicationDbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor, IUserAccessor userAccessor)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(CreateHardwareEquipmentCommand request, CancellationToken cancellationToken)
        {
            var heq = _mapper.Map<HardwareEquipment>(request);

            var comapnyId = _userAccessor.GetCompanyId();
            heq.CompanyId = comapnyId.Value;

            _context.Add(heq);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success(heq.Id);

            return CommandResponse.Failure(500);
        }
    }
}
