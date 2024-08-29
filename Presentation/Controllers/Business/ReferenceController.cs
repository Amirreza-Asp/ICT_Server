using Application.Repositories;
using Application.Services.Interfaces;
using Domain.Dtos.Shared;
using Domain.Entities.Business;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.Business
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReferenceController : ControllerBase
    {
        private readonly IRepository<HardwareEquipment> _heqRepo;
        private readonly IRepository<Domain.Entities.Business.System> _systemRepo;
        private readonly IRepository<Person> _personRepo;
        private readonly IUserAccessor _userAccessor;

        public ReferenceController(IRepository<HardwareEquipment> heqRepo, IRepository<Domain.Entities.Business.System> systemRepo, IRepository<Person> personRepo, IUserAccessor userAccessor)
        {
            _heqRepo = heqRepo;
            _systemRepo = systemRepo;
            _personRepo = personRepo;
            _userAccessor = userAccessor;
        }


        [HttpGet]
        [Route("SelectList")]
        public async Task<List<MultiDataSelect>> GetSelectList(CancellationToken cancellationToken)
        {
            var hardwares = await _heqRepo.GetAllAsync<SelectSummary>(b => b.CompanyId == _userAccessor.GetCompanyId());
            var systems = await _systemRepo.GetAllAsync<SelectSummary>(b => b.CompanyId == _userAccessor.GetCompanyId());
            var persons = await _personRepo.GetAllAsync<SelectSummary>(b => b.CompanyId == _userAccessor.GetCompanyId());

            var data = new List<MultiDataSelect>()
            {
                new MultiDataSelect
                {
                    Title = "تجهیزات سخت افزاری",
                    Data = hardwares.Select(e=>new MultiDataSelectItem{Text = e.Title,Value=e.Title}).ToList()
                },
                new MultiDataSelect
                {
                    Title = "افراد",
                    Data = persons.Select(e=>new MultiDataSelectItem{Text = e.Title,Value=e.Title}).ToList()
                },
                new MultiDataSelect
                {
                    Title = "سامانه ها",
                    Data = systems.Select(e=>new MultiDataSelectItem{Text = e.Title,Value=e.Title}).ToList()
                },
            };

            return data;
        }
    }
}
