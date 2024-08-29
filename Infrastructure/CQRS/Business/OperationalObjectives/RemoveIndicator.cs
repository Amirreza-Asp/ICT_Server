using Domain.Dtos.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CQRS.Business.OperationalObjectives
{
    public class RemoveOperationalObjectiveIndicatorCommand : IRequest<CommandResponse>
    {
        [Required]
        public Guid OperationalObjectiveId { get; set; }

        [Required]
        public Guid IndicatorId { get; set; }
    }

    public class RemoveOperationalObjectiveIndicatorCommandHandler : IRequestHandler<RemoveOperationalObjectiveIndicatorCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public RemoveOperationalObjectiveIndicatorCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(RemoveOperationalObjectiveIndicatorCommand request, CancellationToken cancellationToken)
        {
            var ino =
                await _context.OperationalObjectiveIndicators
                    .Where(b => b.IndicatorId == request.IndicatorId && b.OperationalObjectiveId == request.OperationalObjectiveId)
                    .FirstOrDefaultAsync(cancellationToken);

            if (ino == null)
                return CommandResponse.Success();

            _context.OperationalObjectiveIndicators.Remove(ino);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(500, "مشکل داخلی سرور");
        }
    }
}
