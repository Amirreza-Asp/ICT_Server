using Domain.Entities.Static;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Business
{
    public class Indicator : BaseEntity
    {
        [Required]
        public DateTime FromDate { get; set; }

        [Required]
        public DateTime ToDate { get; set; }

        [Required]
        public IndicatorPeriod Period { get; set; }

        [Required]
        public long GoalValue { get; set; }

        [Required]
        public long InitValue { get; set; }

        public Guid CategoryId { get; set; }
        public Guid TypeId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public IndicatorCategory Category { get; set; }

        [ForeignKey(nameof(TypeId))]
        public IndicatorType Type { get; set; }

        public ICollection<IndicatorProgress> Progresses { get; set; } = new List<IndicatorProgress>();
    }
}
