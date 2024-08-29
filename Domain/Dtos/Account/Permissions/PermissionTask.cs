namespace Domain.Dtos.Account.Permissions
{
    public class PermissionTask
    {
        public PermissionTask(string title)
        {
            Title = title;
            Childs = new List<PermissionTask>();
        }

        public String Title { get; set; }
        public List<PermissionTask> Childs { get; set; }
    }
}
