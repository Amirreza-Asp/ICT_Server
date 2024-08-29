using Domain.Dtos.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CQRS.Business.BigGoals
{
    public class RemoveBigGoalIndicatorCommand : IRequest<CommandResponse>
    {
        [Required]
        public Guid IndicatorId { get; set; }

        [Required]
        public Guid BigGoalId { get; set; }
    }

    public class RemoveBigGoalIndicatorCommandHandler : IRequestHandler<RemoveBigGoalIndicatorCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public RemoveBigGoalIndicatorCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(RemoveBigGoalIndicatorCommand request, CancellationToken cancellationToken)
        {
            var inb =
                await _context.BigGoalIndicators
                    .Where(b => b.IndicatorId == request.IndicatorId && b.BigGoalId == request.BigGoalId)
                    .FirstOrDefaultAsync(cancellationToken);

            if (inb == null)
                return CommandResponse.Success();

            _context.BigGoalIndicators.Remove(inb);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(500, "مشکل داخلی سرور");
        }
    }
}
