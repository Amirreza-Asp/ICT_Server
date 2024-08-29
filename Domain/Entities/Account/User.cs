using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Account
{
    public class User : BaseEntity
    {
        [Required]
        public String NationalId { get; set; }

        public String Name { get; set; }
        public String Family { get; set; }

        public bool IsActive { get; set; } = true;

        public Token Token { get; set; }
        public RefreshToken RefreshToken { get; set; }

        public ICollection<Act> Act { get; set; } = new List<Act>();
    }
}
