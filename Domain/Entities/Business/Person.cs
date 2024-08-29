using Domain.Entities.Account;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Business
{
    public class Person : BaseEntity
    {
        [Required]
        public String Name { get; set; }

        [Required]
        public String Family { get; set; }

        [Required]
        public String JobTitle { get; set; }

        [Required]
        public String Education { get; set; }

        public List<Expertise> Expertises { get; set; } = new List<Expertise>();

        public Guid CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public Company Company { get; set; }

        public ICollection<Transition> Transitions { get; set; }
    }

    public class Expertise
    {
        [Required]
        public String Title { get; set; }
    }
}
