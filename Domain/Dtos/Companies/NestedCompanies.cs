namespace Domain.Dtos.Companies
{
    public class NestedCompanies
    {
        public Guid Id { get; set; }
        public String Title { get; set; }
        public int IndicatorCount { get; set; }
        public int UsersCount { get; set; }
        public String Agent { get; set; }

        public List<NestedCompanies> Childs { get; set; } = new List<NestedCompanies>();
    }
}
