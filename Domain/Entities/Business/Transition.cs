using Domain.SubEntities;
using Domain.Utiltiy;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Business
{
    public class Transition : BaseEntity
    {
        [Required]
        public String Title { get; set; }

        public String Contractor { get; set; }

        [Required]
        public DateTime StartedAt { get; set; }

        [Required]
        public DateTime Deadline { get; set; }

        [NotMapped]
        public int Progress => Calculator.CalcProgress(Indicators.Select(e => e.Indicator));

        [ForeignKey(nameof(Leader))]
        public Guid LeaderId { get; set; }


        [ForeignKey(nameof(OperationalObjective))]
        public Guid OperationalObjectiveId { get; set; }

        [Required]
        public TransitionType Type { get; set; }

        [ForeignKey(nameof(Transition))]
        public Guid? ParentId { get; set; }

        public Transition Parent { get; set; }
        public Person Leader { get; set; }
        public ICollection<TransitionIndicator> Indicators { get; set; } = new List<TransitionIndicator>();
        public ICollection<Transition> Childs { get; set; } = new List<Transition>();
        public List<Financial> Financials { get; set; } = new List<Financial>();
        public OperationalObjective OperationalObjective { get; set; }
    }


}
