namespace Domain.Dtos.Companies
{
    public class CompanySummary
    {
        public Guid Id { get; set; }
        public String Title { get; set; }
        public String Type { get; set; }
        public String Province { get; set; }
        public String City { get; set; }
    }
}
