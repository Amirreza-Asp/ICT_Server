using Domain.Dtos.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CQRS.Business.Strategies
{
    public class UpdateStrategyCommand : IRequest<CommandResponse>
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public String Content { get; set; }
    }

    public class UpdateStrategyCommandHandler : IRequestHandler<UpdateStrategyCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public UpdateStrategyCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(UpdateStrategyCommand request, CancellationToken cancellationToken)
        {
            var strategy =
                await _context.Strategy
                    .Where(b => b.Id == request.Id)
                    .FirstOrDefaultAsync(cancellationToken);

            if (strategy == null)
                return CommandResponse.Failure(400, "راهبرد انتخاب شده در سیستم وجود ندارد");

            strategy.Content = request.Content;

            _context.Strategy.Update(strategy);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}

