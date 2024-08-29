namespace Domain.Dtos.OperationalObjectives
{
    public class OperationalObjectiveSummary
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }

        public String Title { get; set; }

        public DateTime StartedAt { get; set; }

        public DateTime Deadline { get; set; }
        public int ProjectsCount { get; set; }
        public int ActionsCount { get; set; }
        public int IndicatorsCount { get; set; }
        public int Progress { get; set; }

    }
}
