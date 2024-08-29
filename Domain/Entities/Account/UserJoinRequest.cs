using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Account
{
    public class UserJoinRequest : BaseEntity
    {
        [Required]
        public String NationalId { get; set; }

        [Required]
        public String FullName { get; set; }


        public String PhoneNumber { get; set; }

        public Guid? CompanyId { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public Company Company { get; set; }
    }
}
