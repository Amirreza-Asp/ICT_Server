namespace Domain.Dtos.People
{
    public class PersonSummary
    {
        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public String Name { get; set; }

        public String Family { get; set; }

        public String JobTitle { get; set; }

        public String Education { get; set; }

        public List<String> Expertises { get; set; } = new List<String>();
    }
}
