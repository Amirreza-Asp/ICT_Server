using Domain.Dtos.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CQRS.Business.Systems
{
    public class DeleteSystemCommand : IRequest<CommandResponse>
    {
        [Required]
        public Guid Id { get; set; }
    }

    public class DeleteSystemCommandHandler : IRequestHandler<DeleteSystemCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public DeleteSystemCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(DeleteSystemCommand request, CancellationToken cancellationToken)
        {
            var system = await _context.Systems.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (system == null)
                return CommandResponse.Failure(400, "سامانه انتخاب شده در سیستم وجود ندارد");

            _context.Systems.Remove(system);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(500, "ویرایش سامانه با شکست مواجه شد");
        }
    }
}

