using Domain.Dtos.Shared;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CQRS.Business.OperationalObjectives
{
    public class DeleteOperationObjectiveCommand : IRequest<CommandResponse>
    {
        [Required]
        public Guid Id { get; set; }
    }

    public class DeleteOperationObjectiveCommandHandler : IRequestHandler<DeleteOperationObjectiveCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public DeleteOperationObjectiveCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(DeleteOperationObjectiveCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.OperationalObjectives.FindAsync(request.Id);

            if (entity == null)
                return CommandResponse.Success();

            _context.OperationalObjectives.Remove(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(500, "خطای سرور");
        }
    }
}
