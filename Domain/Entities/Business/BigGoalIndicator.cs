using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Business
{
    public class BigGoalIndicator
    {
        public Guid IndicatorId { get; set; }
        public Guid BigGoalId { get; set; }

        [ForeignKey(nameof(BigGoalId))]
        public BigGoal BigGoal { get; set; }
        [ForeignKey(nameof(IndicatorId))]
        public Indicator Indicator { get; set; }
    }
}
