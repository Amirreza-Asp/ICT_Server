using Domain.Dtos.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CQRS.Business.Transitions
{
    public class DeleteTransitionCommand : IRequest<CommandResponse>
    {
        [Required]
        public Guid Id { get; set; }
    }

    public class DeleteTransitionCommandHandler : IRequestHandler<DeleteTransitionCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public DeleteTransitionCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(DeleteTransitionCommand request, CancellationToken cancellationToken)
        {
            var transition = await _context.Transitions.Include(b => b.Financials).FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (transition == null)
                return CommandResponse.Success(200);

            _context.Transitions.Remove(transition);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(500, "خطای سرور");
        }
    }
}
