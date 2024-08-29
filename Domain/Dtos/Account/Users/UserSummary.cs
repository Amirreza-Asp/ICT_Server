namespace Domain.Dtos.Account.Users
{
    public class UserSummary
    {
        public Guid Id { get; set; }
        public String FullName { get; set; }
        public String NationalId { get; set; }
        public String Role { get; set; }
        public Guid RoleId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
