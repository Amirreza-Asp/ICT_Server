namespace Domain.Dtos.Account.Permissions
{
    public class NestedPermissions
    {
        public NestedPermissions(Guid id, String title)
        {
            Id = id;
            Title = title;
            Childs = new List<NestedPermissions>();
        }

        public Guid Id { get; set; }
        public String Title { get; set; }
        public List<NestedPermissions> Childs { get; set; }
    }
}
