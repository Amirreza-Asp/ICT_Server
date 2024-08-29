using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Account
{
    public abstract class Permission : BaseEntity
    {
        [Required]
        public String Title { get; set; }
        public String Discriminator { get; set; }
        public PermissionType Type { get; set; }

        public ICollection<RolePermission> Roles { get; set; } = new List<RolePermission>();
    }

    public class PermissionContainer : Permission
    {
        readonly List<Permission> _childrens = new List<Permission>();
        public ICollection<Permission> Childrens => _childrens;

        public PermissionContainer(string title, PermissionType type)
        {
            Id = Guid.NewGuid();
            Title = title;
            Type = type;
            Discriminator = nameof(PermissionContainer);
        }


        public PermissionContainer(string title, PermissionType type, List<Permission> childrens) : this(Guid.NewGuid(), title, type, childrens)
        {
        }

        public PermissionContainer(Guid id, string title, PermissionType type, List<Permission> childrens)
        {
            GuardAgaintSelfChildren(childrens.ToArray());

            Id = id;
            Title = title;
            Type = type;
            Discriminator = nameof(PermissionContainer);
            _childrens = childrens;
        }

        private PermissionContainer()
        { }

        public void Add(Permission permission)
        {
            GuardAgaintSelfChildren(permission);
            _childrens.Add(permission);
        }

        public void Remove(Permission permission)
        {
            _childrens.Remove(permission);
        }

        private void GuardAgaintSelfChildren(params Permission[] permissions)
        {
            if (permissions.Any(b => b.Id == Id))
                throw new Exception("نمیتوان یک سطح دسترسی را به عنوان فرزند به خودش داد");
        }

    }

    public class PermissionItem : Permission
    {
        public String PageValue { get; protected set; }

        public PermissionItem(string title, PermissionType type, string pageValue)
        {
            Id = Guid.NewGuid();
            Title = title;
            PageValue = pageValue;
            Type = type;
            Discriminator = nameof(PermissionItem);
        }
        private PermissionItem()
        { }
    }

}
