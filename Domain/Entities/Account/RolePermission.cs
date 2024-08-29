using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Account
{
    public class RolePermission : BaseEntity
    {
        [ForeignKey(nameof(Role))]
        public Guid RoleId { get; set; }

        [ForeignKey(nameof(Permission))]
        public Guid PermissionId { get; set; }

        public Role Role { get; set; }
        public Permission Permission { get; set; }
    }
}
