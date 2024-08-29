using Domain.Dtos.Companies;
using Domain.Dtos.Refrences;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.CQRS.Account.Companies
{
    public class CompaniesHardwareEquipentsQuery : IRequest<List<CompanyHardwareEquipments>>
    {
        public List<Guid> Companies { get; set; }
        public String SupportType { get; set; }
    }

    public class CompaniesHardwareEquipentsQueryHandler : IRequestHandler<CompaniesHardwareEquipentsQuery, List<CompanyHardwareEquipments>>
    {
        private readonly ApplicationDbContext _context;

        public CompaniesHardwareEquipentsQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CompanyHardwareEquipments>> Handle(CompaniesHardwareEquipentsQuery request, CancellationToken cancellationToken)
        {
            var data =
                await _context.HardwareEquipment
             .Where(b =>
                     request.Companies.Contains(b.CompanyId) &&
                    (String.IsNullOrWhiteSpace(request.SupportType) || b.SupportType.Contains(request.SupportType.Trim())))
             .Select(op => new CompanyHardwareEquipments
             {
                 CompanyId = op.CompanyId,
                 CompanyName = op.Company.Title,
                 hardwareEquipments = new List<HardwareEquipmentDto>
                 {
                            new HardwareEquipmentDto
                            {
                                Id = op.Id,
                                SupportType = op.SupportType,
                                Count = op.Count,
                                Brand = op.BrandName,
                                Title = op.Title
                            }
                 }
             })
             .ToListAsync(cancellationToken);

            var coo = new List<CompanyHardwareEquipments>();
            data.ForEach(item =>
            {
                if (coo.Any(b => b.CompanyId == item.CompanyId))
                {
                    coo.Find(b => b.CompanyId == item.CompanyId).hardwareEquipments.Add(item.hardwareEquipments.First());
                }
                else
                {
                    coo.Add(item);
                }
            });


            return coo;
        }
    }
}
