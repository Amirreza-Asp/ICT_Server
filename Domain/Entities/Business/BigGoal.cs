using Domain.Utiltiy;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Business
{
    public class BigGoal : BaseEntity
    {
        [Required]
        public String Title { get; set; }

        public String Description { get; set; }

        [Required]
        public DateTime StartedAt { get; set; }

        [Required]
        public DateTime Deadline { get; set; }

        [NotMapped]
        public int Progress => Calculator.CalcProgress(Indicators.Select(b => b.Indicator));

        public ICollection<ProgramBigGoal> Programs { get; set; }

        public ICollection<OperationalObjective> OperationalObjectives { get; set; } = new List<OperationalObjective>();
        public ICollection<BigGoalIndicator> Indicators { get; set; } = new List<BigGoalIndicator>();
    }
}
