using Domain.Dtos.Account.Users;

namespace Domain.Dtos.Companies
{
    public class CompanyUsers
    {
        public Guid CompanyId { get; set; }
        public String CompanyName { get; set; }
        public List<UserListDto> Users { get; set; }
    }
}
