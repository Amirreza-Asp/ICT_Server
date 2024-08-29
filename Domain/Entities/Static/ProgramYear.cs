using System.ComponentModel.DataAnnotations;
using Domain.Entities.Business;

namespace Domain.Entities.Static
{
    public class ProgramYear : BaseEntity
    {
        [Required]
        public string Year { get; set; }

        public ICollection<BigGoal> BigGoals { get; set; } = new List<BigGoal>();
    }
}
