using Domain.Entities.Account;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Business
{
    public class Program : BaseEntity
    {
        [Required]
        public String Title { get; set; }

        [Required]
        public DateTime StartedAt { get; set; }

        [Required]
        public DateTime EndAt { get; set; }

        [Required]
        public String Description { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [ForeignKey(nameof(Company))]
        public Guid CompanyId { get; set; }
        public Company Company { get; set; }

        public ICollection<ProgramBigGoal> BigGoals { get; set; }
        public Perspective Perspective { get; set; }
    }
}
