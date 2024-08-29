using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Account
{
    public class RefreshToken : BaseEntity
    {
        [Required]
        public Guid Value { get; set; }

        [Required]
        public DateTime Expiration { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
    }
}
