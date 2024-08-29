using Domain.Dtos.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CQRS.Business.SOWTs
{
    public class DeleteSWOTCommand : IRequest<CommandResponse>
    {
        [Required]
        public Guid Id { get; set; }
    }

    public class DeleteSWOTCommandHandler : IRequestHandler<DeleteSWOTCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public DeleteSWOTCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(DeleteSWOTCommand request, CancellationToken cancellationToken)
        {
            var sowt =
                await _context.SWOT
                    .Where(b => b.Id == request.Id)
                    .FirstOrDefaultAsync(cancellationToken);

            if (sowt == null)
                return CommandResponse.Failure(400, "آیتم مورد نظر در سیستم وجود ندارد");

            _context.SWOT.Remove(sowt);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
