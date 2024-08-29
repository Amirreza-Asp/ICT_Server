using Domain.Dtos.Companies;
using Domain.Dtos.Shared;
using Domain.Entities.Account;
using Domain.Queries.Shared;

namespace Application.Repositories
{
    public interface ICompanyRepository : IRepository<Company>
    {
        Task<List<String>> GetProvincesAsync(CancellationToken cancellationToken);
        Task<List<String>> GetProvinceCitiesAsync(String province, CancellationToken cancellationToken);
        Task<List<String>> GetTypesAsync(CancellationToken cancellationToken);
        Task<NestedCompanies> GetNestedAsync(CancellationToken cancellation);
        Task<List<SelectSummary>> GetSelectListUsersAsync(CancellationToken cancellationToken);
        Task<List<CompanySummary>> GetChildsCompaniesPagenationAsync(Guid id, GridQuery query, CancellationToken cancellationToken);
    }
}
