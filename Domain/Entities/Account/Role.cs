using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Account
{
    public class Role : BaseEntity
    {
        [Required]
        public String Title { get; set; }
        public String Description { get; set; }

        public ICollection<RolePermission> Permissions { get; set; } = new List<RolePermission>();
        public ICollection<Act> Acts { get; set; } = new List<Act>();
    }
}
