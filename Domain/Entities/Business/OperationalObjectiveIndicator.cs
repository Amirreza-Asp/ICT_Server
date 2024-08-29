using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Business
{
    public class OperationalObjectiveIndicator
    {
        public Guid OperationalObjectiveId { get; set; }
        public Guid IndicatorId { get; set; }


        [ForeignKey(nameof(OperationalObjectiveId))]
        public OperationalObjective OperationalObjective { get; set; }

        [ForeignKey(nameof(IndicatorId))]
        public Indicator Indicator { get; set; }
    }
}
