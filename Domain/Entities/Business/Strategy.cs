using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Business
{
    public class Strategy : BaseEntity
    {
        [Required]
        public String Content { get; set; }

        [ForeignKey(nameof(Program))]
        public Guid ProgramId { get; set; }
        public Program Program { get; set; }
    }
}
