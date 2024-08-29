namespace Domain.Dtos.Transitions
{
    public class TransitionSummary
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }

        public String Title { get; set; }

        public DateTime StartedAt { get; set; }

        public DateTime Deadline { get; set; }

        public int ProjectsCount { get; set; }
        public int ActionsCount { get; set; }
        public int IndicatorsCount { get; set; }


        public Guid OperationalObjectiveId { get; set; }

        public int Progress { get; set; }
    }
}
