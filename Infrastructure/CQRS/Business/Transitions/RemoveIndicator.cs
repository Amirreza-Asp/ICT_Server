using Domain.Dtos.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CQRS.Business.Transitions
{
    public class RemoveTransitionIndicatorCommand : IRequest<CommandResponse>
    {
        [Required]
        public Guid Id { get; set; }
    }

    public class RemoveTransitionIndicatorCommandHandler : IRequestHandler<RemoveTransitionIndicatorCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public RemoveTransitionIndicatorCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(RemoveTransitionIndicatorCommand request, CancellationToken cancellationToken)
        {
            var inp =
                await _context.TransitionIndicators
                    .Where(b => b.Id == request.Id)
                    .FirstOrDefaultAsync(cancellationToken);

            if (inp == null)
                return CommandResponse.Success();

            _context.TransitionIndicators.Remove(inp);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(500, "مشکل داخلی سرور");
        }
    }
}
