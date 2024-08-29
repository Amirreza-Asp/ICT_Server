using Domain.Dtos.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CQRS.Business.OperationalObjectives
{
    public class UpdateOperationalObjectiveCommand : IRequest<CommandResponse>
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public String Title { get; set; }

        public String Description { get; set; }

        [Required]
        public DateTime GuaranteedFulfillmentAt { get; set; }

        [Required]
        public DateTime Deadline { get; set; }
    }

    public class UpdateOperationalObjectiveCommandHandler : IRequestHandler<UpdateOperationalObjectiveCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public UpdateOperationalObjectiveCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(UpdateOperationalObjectiveCommand request, CancellationToken cancellationToken)
        {
            var operationaObjective = await _context.OperationalObjectives.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (operationaObjective == null)
                return CommandResponse.Failure(400, "هدف عملیاتی انتخاب شده در سیستم وجود ندارد");

            operationaObjective.Title = request.Title;
            operationaObjective.Description = request.Description;
            operationaObjective.GuaranteedFulfillmentAt = request.GuaranteedFulfillmentAt;
            operationaObjective.Deadline = request.Deadline;

            _context.OperationalObjectives.Update(operationaObjective);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(500, "مشکل داخلی سرور");
        }
    }
}
