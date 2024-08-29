using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Account
{
    public class Act
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(Company))]
        public Guid CompanyId { get; set; }

        [ForeignKey(nameof(Role))]
        public Guid RoleId { get; set; }

        public Role Role { get; set; }
        public Company Company { get; set; }
        public User User { get; set; }
    }
}
