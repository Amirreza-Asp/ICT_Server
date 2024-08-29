namespace Domain.Dtos.Account.Users
{
    public class UserRequestsSummary
    {
        public Guid Id { get; set; }
        public String NationalId { get; set; }
        public String FullName { get; set; }
        public DateTime RequestTime { get; set; }
        public String PhoneNumber { get; set; }
    }
}
