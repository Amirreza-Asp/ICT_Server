using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Business
{
    public class SWOT : BaseEntity
    {
        [Required]
        public String Content { get; set; }

        [Required]
        public SWOTType Type { get; set; }

        [ForeignKey(nameof(Program))]
        public Guid ProgramId { get; set; }
        public Program Program { get; set; }
    }

    public enum SWOTType
    {
        Strengths = 10,
        Weaknesses = 20,
        Opportunities = 30,
        Threats = 40
    }
}
