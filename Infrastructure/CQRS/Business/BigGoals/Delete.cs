using Domain.Dtos.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CQRS.Business.BigGoals
{
    public class DeleteBigGoalCommand : IRequest<CommandResponse>
    {
        [Required]
        public Guid Id { get; set; }
    }

    public class DeleteBigGoalCommandHandler : IRequestHandler<DeleteBigGoalCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public DeleteBigGoalCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(DeleteBigGoalCommand request, CancellationToken cancellationToken)
        {
            var bigGoal =
                await _context.BigGoals
                    .Where(b => b.Id == request.Id)
                    .FirstOrDefaultAsync(cancellationToken);

            if (bigGoal == null)
                return CommandResponse.Success();

            _context.BigGoals.Remove(bigGoal);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(500, "مشکل داخلی سرور");
        }
    }
}

