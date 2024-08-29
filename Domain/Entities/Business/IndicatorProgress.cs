using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Business
{
    public class IndicatorProgress : BaseEntity
    {
        [Required]
        public long Value { get; set; }

        [Required]
        public DateTime ProgressTime { get; set; }

        public Guid IndicatorId { get; set; }
        [ForeignKey(nameof(IndicatorId))]
        public Indicator Indicator { get; set; }
    }
}
