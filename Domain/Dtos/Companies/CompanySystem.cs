using Domain.Dtos.Refrences;

namespace Domain.Dtos.Companies
{
    public class CompanySystem
    {
        public Guid CompanyId { get; set; }
        public String CompanyName { get; set; }
        public List<SystemDto> Systems { get; set; }
    }
}
