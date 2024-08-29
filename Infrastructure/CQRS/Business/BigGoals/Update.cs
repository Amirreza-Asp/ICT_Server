using Domain.Dtos.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CQRS.Business.BigGoals
{
    public class UpdateBigGoalCommand : IRequest<CommandResponse>
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime StartedAt { get; set; }

        [Required]
        public DateTime Deadline { get; set; }

        public Guid? ProgramYearId { get; set; }
    }

    public class UpdateBigGoalCommandHandler : IRequestHandler<UpdateBigGoalCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public UpdateBigGoalCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(UpdateBigGoalCommand request, CancellationToken cancellationToken)
        {
            var bigGoal = await _context.BigGoals.FirstOrDefaultAsync(b => b.Id == request.Id);

            if (bigGoal == null)
                return CommandResponse.Failure(400, "هدف کلان انتخاب شده در سیستم وجود ندارد");

            bigGoal.Title = request.Title;
            bigGoal.Description = request.Description;
            bigGoal.Deadline = request.Deadline;
            bigGoal.StartedAt = request.StartedAt;

            _context.BigGoals.Update(bigGoal);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(500, "مشکل داخلی سرور");
        }
    }
}
