using Domain.Utiltiy;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Business
{
    public class OperationalObjective : BaseEntity
    {
        [Required]
        public String Title { get; set; }

        public String Description { get; set; }

        [Required]
        public DateTime GuaranteedFulfillmentAt { get; set; }

        [Required]
        public DateTime Deadline { get; set; }

        public long Budget { get; set; }

        [ForeignKey(nameof(BigGoal))]
        public Guid BigGoalId { get; set; }

        [NotMapped]
        public int Progress => Calculator.CalcProgress(Indicators.Select(e => e.Indicator));

        public BigGoal BigGoal { get; set; }
        public ICollection<Transition> Transitions { get; set; } = new List<Transition>();
        public ICollection<OperationalObjectiveIndicator> Indicators { get; set; } = new List<OperationalObjectiveIndicator>();
    }
}
