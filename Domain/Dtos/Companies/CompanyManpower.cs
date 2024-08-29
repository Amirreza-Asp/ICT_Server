using Domain.Dtos.People;

namespace Domain.Dtos.Companies
{
    public class CompanyManpower
    {
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; }
        public List<ManpowerDto> Manpowers { get; set; }
    }
}
