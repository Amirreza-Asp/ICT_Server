namespace Domain.Dtos.Account.Users
{
    public class UserListDto
    {
        public Guid Id { get; set; }
        public String FullName { get; set; }
        public String NationalId { get; set; }
        public String PhoneNumber { get; set; }
        public bool IsAdmin { get; set; }
    }
}
