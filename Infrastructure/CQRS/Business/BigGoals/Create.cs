using Application.Repositories;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Dtos.Shared;
using Domain.Entities.Business;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CQRS.Business.BigGoals
{
    public class CreateBigGoalCommand : IRequest<CommandResponse>
    {
        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime StartedAt { get; set; }

        [Required]
        public DateTime Deadline { get; set; }
    }

    public class CreateBigGoalCommandHandler : IRequestHandler<CreateBigGoalCommand, CommandResponse>
    {
        private readonly IRepository<BigGoal> _repo;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserAccessor _userAccessor;
        private readonly ApplicationDbContext _context;

        public CreateBigGoalCommandHandler(IRepository<BigGoal> repo, IMapper mapper, IHttpContextAccessor httpContextAccessor, IUserAccessor userAccessor, ApplicationDbContext context)
        {
            _repo = repo;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _userAccessor = userAccessor;
            _context = context;
        }

        public async Task<CommandResponse> Handle(CreateBigGoalCommand request, CancellationToken cancellationToken)
        {
            if (request.Deadline < request.StartedAt)
                return CommandResponse.Failure(400, "تاریخ شروع نمیتواند از مهلت انجام بیشتر باشد");

            var comapnyId = _userAccessor.GetCompanyId();
            var activeProgram = await _context.Program.Where(b => b.CompanyId == comapnyId.Value && b.IsActive).FirstOrDefaultAsync();

            if (activeProgram == null)
                return CommandResponse.Failure(400, "هیچ برنامه ای انتخاب نشده است");

            var bigGoal = _mapper.Map<BigGoal>(request);
            var programBigGoal = new ProgramBigGoal(activeProgram.Id, bigGoal.Id);


            _repo.Create(bigGoal);
            _context.ProgramBigGoal.Add(programBigGoal);

            if (await _repo.SaveAsync(cancellationToken))
            {
                return CommandResponse.Success(bigGoal.Id);
            }

            return CommandResponse.Failure(500);
        }
    }
}
