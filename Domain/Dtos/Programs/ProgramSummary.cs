namespace Domain.Dtos.Programs
{
    public class ProgramSummary
    {
        public Guid Id { get; set; }
        public String Title { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime EndAt { get; set; }
        public int BigGoalsCount { get; set; }
        public bool IsActive { get; set; }
    }
}
