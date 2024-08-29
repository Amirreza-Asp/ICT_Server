using Domain.Dtos.Account.Permissions;

namespace Domain.Dtos.Account.User
{
    public class UserProfile
    {
        public String NationalId { get; set; }
        public String FullName { get; set; }
        public string Mobile { get; set; }
        public string Gender { get; set; }
        public String Company { get; set; }
        public Guid? CompanyId { get; set; }
        public List<PermissionSummary> Permissions { get; set; }
    }
}
