using Domain.Dtos.Refrences;

namespace Domain.Dtos.Companies
{
    public class CompanyHardwareEquipments
    {
        public Guid CompanyId { get; set; }
        public String CompanyName { get; set; }
        public List<HardwareEquipmentDto> hardwareEquipments { get; set; }
    }
}
