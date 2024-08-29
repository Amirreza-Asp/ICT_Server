using Domain.Dtos.Transitions;

namespace Domain.Dtos.Companies
{
    public class CompanyProjects
    {
        public Guid CompanyId { get; set; }
        public String CompanyName { get; set; }
        public List<TransitionListDto> Projects { get; set; }
    }
}
