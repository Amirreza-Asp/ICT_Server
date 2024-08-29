using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Account
{
    public class Token : BaseEntity
    {
        [Required]
        public String HashValue { get; set; }

        [Required]
        public DateTime Expiration { get; set; }

        [Required]
        public String Ip { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [ForeignKey(nameof(UserId))]
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
