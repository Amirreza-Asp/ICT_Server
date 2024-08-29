using Application.Services.Interfaces;
using Domain.Dtos.Shared;
using Domain.Entities.Business;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CQRS.Business.Strategies
{
    public class CreateStrategyCommand : IRequest<CommandResponse>
    {
        [Required]
        public String Content { get; set; }
    }


    public class CreateStrategyCommandHandler : IRequestHandler<CreateStrategyCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserAccessor _userAccessor;

        public CreateStrategyCommandHandler(ApplicationDbContext context, IUserAccessor userAccessor)
        {
            _context = context;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(CreateStrategyCommand request, CancellationToken cancellationToken)
        {
            var companyId = _userAccessor.GetCompanyId();

            var activeProgramId =
                await _context.Program
                    .Where(b => b.IsActive && b.CompanyId == companyId.Value)
                    .Select(b => b.Id)
                    .FirstOrDefaultAsync(cancellationToken);

            if (activeProgramId == default)
                return CommandResponse.Failure(400, "هیچ برنامه ای انتخاب نشده است");

            var strategy = new Strategy
            {
                Id = Guid.NewGuid(),
                ProgramId = activeProgramId,
                Content = request.Content
            };

            _context.Strategy.Add(strategy);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success(strategy.Id);

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
