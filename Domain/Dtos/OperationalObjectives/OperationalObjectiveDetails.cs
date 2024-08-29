using Domain.Dtos.Shared;

namespace Domain.Dtos.OperationalObjectives
{
    public class OperationalObjectiveDetails
    {
        public Guid Id { get; set; }
        public String Title { get; set; }

        public String Description { get; set; }
        public bool Active { get; set; }

        public DateTime GuaranteedFulfillmentAt { get; set; }
        public DateTime Deadline { get; set; }
        public int Progress { get; set; }

        public int ProjectsCount { get; set; }
        public int PracticalActionsCount { get; set; }

        public List<IndicatorCard> Indicators { get; set; } = new List<IndicatorCard>();
    }
}
