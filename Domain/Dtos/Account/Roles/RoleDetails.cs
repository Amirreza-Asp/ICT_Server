namespace Domain.Dtos.Account.Roles
{
    public class RoleDetails
    {
        public Guid Id { get; set; }
        public String Title { get; set; }
        public String Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<Guid> Permissios { get; set; }
    }
}
