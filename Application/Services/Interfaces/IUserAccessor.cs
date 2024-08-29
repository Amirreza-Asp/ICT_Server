namespace Application.Services.Interfaces
{
    public interface IUserAccessor
    {
        Guid? GetCompanyId();
        String GetNationalId();
        Guid RoleId();
    }
}
