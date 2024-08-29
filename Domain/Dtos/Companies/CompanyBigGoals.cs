using Domain.Dtos.BigGoals;

namespace Domain.Dtos.Companies
{
    public class CompanyBigGoals
    {
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; }
        public List<BigGoalsListDto> BigGoals { get; set; }
    }
}
