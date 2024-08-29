using Domain.Entities.Account;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Business
{
    public class System : BaseEntity
    {
        [Required]
        public String Title { get; set; }

        public String Description { get; set; }

        [Required]
        public String ProgrammingLanguage { get; set; }

        [Required]
        public String Database { get; set; }

        [Required]
        public String Framework { get; set; }

        [Required]
        public String Development { get; set; }

        [Required]
        public String BuildInCompany { get; set; }

        [Required]
        public String OS { get; set; }

        [Required]
        public String SupportType { get; set; }


        public Guid CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public Company Company { get; set; }
    }
}
