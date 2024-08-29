using AutoMapper;
using Domain.Dtos.Shared;
using Domain.Entities.Business;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CQRS.Business.OperationalObjectives
{
    public class CreateOperationalObjectiveCommand : IRequest<CommandResponse>
    {
        [Required]
        public String Title { get; set; }

        public String Description { get; set; }

        [Required]
        public DateTime GuaranteedFulfillmentAt { get; set; }

        [Required]
        public DateTime Deadline { get; set; }


        [Required]
        public Guid BigGoalId { get; set; }
    }

    public class CreateOperationalObjectiveCommandHandler : IRequestHandler<CreateOperationalObjectiveCommand, CommandResponse>
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        public CreateOperationalObjectiveCommandHandler(IMapper mapper, ApplicationDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<CommandResponse> Handle(CreateOperationalObjectiveCommand request, CancellationToken cancellationToken)
        {
            if (!_context.BigGoals.Any(b => b.Id == request.BigGoalId))
                return CommandResponse.Failure(400, "هدف کلان انتخاب شده نامعتبر است");


            var operationalObjective = _mapper.Map<OperationalObjective>(request);

            _context.Add(operationalObjective);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                return CommandResponse.Success(operationalObjective.Id);
            }

            return CommandResponse.Failure(500);
        }
    }
}
