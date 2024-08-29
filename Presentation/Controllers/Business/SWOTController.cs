using Application.Repositories;
using Application.Services.Interfaces;
using Domain;
using Domain.Dtos.Shared;
using Domain.Dtos.SWOTs;
using Domain.Entities.Business;
using Domain.Queries.Shared;
using Infrastructure.CQRS.Business.SOWTs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.CustomeAttributes;

namespace Presentation.Controllers.Business
{
    [Route("api/[controller]")]
    [ApiController]
    public class SWOTController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IRepository<SWOT> _sowtRepository;
        private readonly IUserAccessor _userAccessor;

        public SWOTController(IMediator mediator, IRepository<SWOT> sowtRepository, IUserAccessor userAccessor)
        {
            _mediator = mediator;
            _sowtRepository = sowtRepository;
            _userAccessor = userAccessor;
        }

        [Route("GetAll")]
        [HttpPost]
        [AccessControl(PermissionsSD.QuerySWOT)]
        public async Task<List<SWOTSummary>> GetAll([FromBody] GridQuery query, CancellationToken cancellationToken)
        {
            var companyId = _userAccessor.GetCompanyId().Value;
            return
                await _sowtRepository.GetAllAsync<SWOTSummary>(query, b => b.Program.CompanyId == companyId && b.Program.IsActive, include: null, cancellationToken);
        }

        [Route("Create")]
        [HttpPost]
        [AccessControl(PermissionsSD.CommandSWOT)]
        public async Task<CommandResponse> Create([FromBody] CreateSWOTCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }

        [Route("Remove")]
        [HttpDelete]
        [AccessControl(PermissionsSD.CommandSWOT)]
        public async Task<CommandResponse> Remove([FromQuery] DeleteSWOTCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }

    }
}
