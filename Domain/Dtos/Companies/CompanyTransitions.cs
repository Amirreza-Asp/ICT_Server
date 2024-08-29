using Domain.Dtos.Transitions;

namespace Domain.Dtos.Companies
{
    public class CompanyTransitions
    {
        public Guid CompanyId { get; set; }
        public String CompanyName { get; set; }
        public List<TransitionListDto> PracticalActions { get; set; }
    }
}
