using Domain.Dtos.People;
using Domain.Entities.Business;

namespace Domain.Dtos.Refrences
{
    public class Refrences
    {
        public List<PersonSummary> Persons { get; set; }
        public List<HardwareEquipment> HardwareEquipments { get; set; }
        public List<SystemDetails> Systems { get; set; }
    }
}
