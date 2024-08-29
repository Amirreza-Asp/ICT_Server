using Domain.Dtos.Shared;
using Domain.Entities.Business;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CQRS.Business.OperationalObjectives
{
    public class AddOperationalObjectiveIndicatorCommand : IRequest<CommandResponse>
    {
        [Required]
        public Guid OperationalObjectiveId { get; set; }

        [Required]
        public String Title { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int InitValue { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int GoalValue { get; set; }

        [Required]
        public DateTime FromDate { get; set; }

        [Required]
        public DateTime ToDate { get; set; }

        [Required]
        public int Period { get; set; }

        [Required]
        public Guid CategoryId { get; set; }

        [Required]
        public Guid TypeId { get; set; }
    }

    public class AddOperationalObjectiveIndicatorCommandHandler : IRequestHandler<AddOperationalObjectiveIndicatorCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public AddOperationalObjectiveIndicatorCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(AddOperationalObjectiveIndicatorCommand request, CancellationToken cancellationToken)
        {
            if (!_context.OperationalObjectives.Any(b => b.Id == request.OperationalObjectiveId))
                return CommandResponse.Failure(400, "هدف عملیاتی انتخاب شده در سیستم وجود ندارد");

            if (!_context.IndicatorCategories.Any(b => b.Id == request.CategoryId))
                return CommandResponse.Failure(400, "طبقه بندی شاخص انتخاب شده در سیستم وجود ندارد");

            if (!_context.IndicatorTypes.Any(b => b.Id == request.TypeId))
                return CommandResponse.Failure(400, "واحد شاخص انتخاب شده در سیستم وجود ندارد");

            if (request.FromDate > request.ToDate)
                return CommandResponse.Failure(400, "تاریخ شروع شاخص نمیتواند از تاریخ پایان بزرگتر باشد");

            var indicator = new Indicator
            {
                Id = Guid.NewGuid(),
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                CategoryId = request.CategoryId,
                TypeId = request.TypeId,
                GoalValue = request.GoalValue,
                InitValue = request.InitValue,
                Period = (IndicatorPeriod)request.Period
            };

            var inp = new OperationalObjectiveIndicator { IndicatorId = indicator.Id, OperationalObjectiveId = request.OperationalObjectiveId };

            _context.Indicators.Add(indicator);
            _context.OperationalObjectiveIndicators.Add(inp);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success(indicator.Id);

            return CommandResponse.Failure(500, "مشکل داخلی سرور");
        }
    }
}
