using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Business
{
    public class TransitionIndicator
    {
        [Key]
        public Guid Id { get; set; }

        public Guid TransitionId { get; set; }
        public Guid IndicatorId { get; set; }


        [ForeignKey(nameof(TransitionId))]
        public Transition Transition { get; set; }

        [ForeignKey(nameof(IndicatorId))]
        public Indicator Indicator { get; set; }
    }
}
