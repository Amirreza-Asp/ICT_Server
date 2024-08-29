using Domain.Dtos.Shared;

namespace Domain.Dtos.BigGoals
{
    public class BigGoalDetails
    {
        public Guid Id { get; set; }

        public String Title { get; set; }

        public String Description { get; set; }

        public DateTime StartedAt { get; set; }

        public DateTime Deadline { get; set; }

        public int OperationalObjectiveCount { get; set; }

        public int Progress { get; set; }

        public String ProgramYear { get; set; }

        public ICollection<IndicatorCard> Indicators { get; set; } = new List<IndicatorCard>();
    }
}
