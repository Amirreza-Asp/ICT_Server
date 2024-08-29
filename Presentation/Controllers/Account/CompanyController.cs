using Application.Repositories;
using Application.Services.Interfaces;
using Domain;
using Domain.Dtos.Companies;
using Domain.Dtos.Shared;
using Domain.Queries.Shared;
using Infrastructure.CQRS.Account.Companies;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.CustomeAttributes;

namespace Presentation.Controllers.Account
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyRepository _repo;
        private readonly IMediator _mediator;
        private readonly IUserAccessor _userAccessor;

        public CompanyController(IMediator mediator, ICompanyRepository repo, IUserAccessor userAccessor)
        {
            _mediator = mediator;
            _repo = repo;
            _userAccessor = userAccessor;
        }


        [Route("GetNestedChilds")]
        [HttpGet]
        [AccessControl(PermissionsSD.QueryCompany)]
        public async Task<NestedCompanies> GetNestedChilds(CancellationToken cancellationToken)
        {
            return await _repo.GetNestedAsync(cancellationToken);
        }

        [Route("GetPagenationChilds")]
        [HttpPost]
        [AccessControl(PermissionsSD.QueryCompany)]
        public async Task<List<CompanySummary>> GetPagenationChilds([FromBody] GridQuery query, CancellationToken cancellationToken)
        {
            var companyId = _userAccessor.GetCompanyId();
            return
                await _repo.GetAllAsync<CompanySummary>(query, b => b.ParentId == companyId.Value, include: null, cancellationToken);
        }

        [Route("AddIndicator")]
        [HttpPost]
        [AccessControl(PermissionsSD.CommandCompany)]
        public async Task<CommandResponse> AddIndicator([FromBody] AddCompanyIndicatorCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }

        [Route("Delete")]
        [HttpDelete]
        [AccessControl(PermissionsSD.CommandCompany)]
        public async Task<CommandResponse> Remove([FromQuery] RemoveCompanyCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command);


        [Route("Create")]
        [HttpPost]
        [AccessControl(PermissionsSD.CommandCompany)]
        public async Task<CommandResponse> Create([FromBody] CreateCompanyCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);


        [Route("AddUser")]
        [HttpPost]
        [AccessControl(PermissionsSD.CommandCompany)]
        public async Task<CommandResponse> AddUser([FromBody] AddUserToCompanyCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);

        [Route("SelectListUsers")]
        [HttpGet]
        public async Task<List<SelectSummary>> SelectListUsers(CancellationToken cancellationToken)
        {
            return await _repo.GetSelectListUsersAsync(cancellationToken);
        }

        [Route("GetAll")]
        [HttpPost]
        [AccessControl(PermissionsSD.QueryCompany)]
        public async Task<ListActionResult<CompanySummary>> GetAll([FromBody] GridQuery query, CancellationToken cancellationToken)
        {
            return await _repo.GetAllAsync<CompanySummary>(query, cancellationToken);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("SelectListCompanies")]
        public async Task<List<SelectSummary>> SelectListCompanies([FromQuery] String search = "", [FromQuery] int page = 1, [FromQuery] int size = 10, CancellationToken cancellationToken = default)
        {

            var gridQuery = new GridQuery
            {
                Page = page,
                Size = size,
                Filters = new List<FilterModel> { new FilterModel { column = "title", value = search == null ? "" : search } }
            };

            var data = await _repo.GetAllAsync<SelectSummary>(gridQuery, cancellationToken);
            return data.Data;
        }

        [Route("BigGoals")]
        [HttpPost]
        [AccessControl(PermissionsSD.FilterCompany)]
        public async Task<List<CompanyBigGoals>> BigGoals([FromBody] CompanyBigGoalsQuery query, CancellationToken cancellationToken)
        {
            return await _mediator.Send(query, cancellationToken);
        }

        [Route("OperationalObjective")]
        [HttpPost]
        [AccessControl(PermissionsSD.FilterCompany)]
        public async Task<List<CompanyOperationalObjectives>> OperationalObjectives([FromBody] OperationalObjectiveQuery query, CancellationToken cancellation)
        {
            return await _mediator.Send(query, cancellation);
        }

        [Route("Projects")]
        [HttpPost]
        [AccessControl(PermissionsSD.FilterCompany)]
        public async Task<List<CompanyProjects>> Projects([FromBody] CompanyProjectsQuery query, CancellationToken cancellationToken)
        {
            return await _mediator.Send(query, cancellationToken);
        }

        [Route("PracticalActions")]
        [HttpPost]
        [AccessControl(PermissionsSD.FilterCompany)]
        public async Task<List<CompanyTransitions>> PracticalActions([FromBody] CompanyPracticalActionQuery query, CancellationToken cancellationToken)
        {
            return await _mediator.Send(query, cancellationToken);
        }

        [Route("Manpowers")]
        [HttpPost]
        [AccessControl(PermissionsSD.FilterCompany)]
        public async Task<List<CompanyManpower>> Manpowers([FromBody] CompaniesManpowerQuery query, CancellationToken cancellationToken)
        {
            return await _mediator.Send(query, cancellationToken);
        }

        [Route("HardwareEquipments")]
        [HttpPost]
        [AccessControl(PermissionsSD.FilterCompany)]
        public async Task<List<CompanyHardwareEquipments>> HardwareEquipments([FromBody] CompaniesHardwareEquipentsQuery query, CancellationToken cancellationToken)
        {
            return await _mediator.Send(query, cancellationToken);
        }

        [Route("Systems")]
        [HttpPost]
        [AccessControl(PermissionsSD.FilterCompany)]
        public async Task<List<CompanySystem>> Systems([FromBody] CompaniesSystemQuery query, CancellationToken cancellationToken)
        {
            return await _mediator.Send(query, cancellationToken);
        }

        [Route("Users")]
        [HttpPost]
        [AccessControl(PermissionsSD.FilterCompany)]
        public async Task<List<CompanyUsers>> Users([FromBody] CompaniesUsersQuery query, CancellationToken cancellationToken)
        {
            return await _mediator.Send(query, cancellationToken);
        }


        [Route("Provinces")]
        [HttpGet]
        [AccessControl(PermissionsSD.FilterCompany)]
        public async Task<List<String>> GetProvices(CancellationToken cancellationToken)
        {
            return await _repo.GetProvincesAsync(cancellationToken);
        }

        [Route("ProvinceCities")]
        [HttpGet]
        [AccessControl(PermissionsSD.FilterCompany)]
        public async Task<List<String>> GetProvinceCities([FromQuery] String province, CancellationToken cancellationToken)
        {
            return await _repo.GetProvinceCitiesAsync(province, cancellationToken);
        }

        [Route("Types")]
        [HttpGet]
        [AccessControl(PermissionsSD.FilterCompany)]
        public async Task<List<String>> GetTypes(CancellationToken cancellationToken)
        {
            return await _repo.GetTypesAsync(cancellationToken);
        }

    }
}
