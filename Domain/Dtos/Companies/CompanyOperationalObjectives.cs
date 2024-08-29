using Domain.Dtos.OperationalObjectives;

namespace Domain.Dtos.Companies
{
    public class CompanyOperationalObjectives
    {
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; }
        public List<OperationalObjectiveListDto> OperationalObjecives { get; set; }
    }
}
