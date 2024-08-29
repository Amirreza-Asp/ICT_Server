using Domain.Dtos.Shared;

namespace Domain.Dtos.Transitions
{
    public class TransitionDetails
    {
        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public String Title { get; set; }

        public String Contractor { get; set; }

        public DateTime StartedAt { get; set; }

        public DateTime Deadline { get; set; }
        public String Type { get; set; }


        public List<String> Financials { get; set; } = new List<String>();

        public Guid LeaderId { get; set; }
        public String LeaderName { get; set; }

        public Guid OperationalObjectiveId { get; set; }
        public String OperationalObjectiveTitle { get; set; }

        public List<IndicatorCard> Indicators { get; set; }
    }
}
