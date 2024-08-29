using Domain.Dtos.Shared;
using Domain.Entities.Business;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CQRS.Business.Indicators
{
    public class AddIndicatorProgressCommand : IRequest<CommandResponse>
    {
        [Required]
        public Guid IndicatorId { get; set; }

        [Range(0, long.MaxValue)]
        public long Value { get; set; }

        [Required]
        public DateTime ProgressTime { get; set; }
    }

    public class AddInidcatorProgressCommandHandler : IRequestHandler<AddIndicatorProgressCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public AddInidcatorProgressCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(AddIndicatorProgressCommand request, CancellationToken cancellationToken)
        {
            if (!await _context.Indicators.AnyAsync(b => b.Id == request.IndicatorId))
                return CommandResponse.Failure(400, "شاخص انتخاب شده وچود ندارد");

            var indicatorProgress = new IndicatorProgress
            {
                IndicatorId = request.IndicatorId,
                Id = Guid.NewGuid(),
                ProgressTime = request.ProgressTime,
                Value = request.Value
            };

            _context.IndicatorProgresses.Add(indicatorProgress);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success(indicatorProgress.Id);

            return CommandResponse.Failure(500, "مشکل داخلی سرور");
        }
    }
}
