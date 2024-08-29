using Domain.Dtos.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CQRS.Static.IndicatorTypes
{
    public class RemoveIndicatorTypeCommand : IRequest<CommandResponse>
    {
        [Required]
        public Guid Id { get; set; }
    }

    public class RemoveIndicatorCommandHandler : IRequestHandler<RemoveIndicatorTypeCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public RemoveIndicatorCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(RemoveIndicatorTypeCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.IndicatorTypes.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (entity == null)
                return CommandResponse.Success();

            _context.IndicatorTypes.Remove(entity);

            if (await _context.SaveChangesAsync() > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(500, "مشکل داخلی سرور");
        }
    }
}
