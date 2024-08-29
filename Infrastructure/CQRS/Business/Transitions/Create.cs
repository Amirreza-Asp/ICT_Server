using AutoMapper;
using Domain.Dtos.Shared;
using Domain.Entities.Business;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CQRS.Business.Transitions
{
    public class CreateTransitionCommand : IRequest<CommandResponse>
    {
        [Required]
        public String Title { get; set; }

        public String Contractor { get; set; }


        [Required]
        public DateTime StartedAt { get; set; }

        [Required]
        public DateTime Deadline { get; set; }

        public List<String> Financials { get; set; } = new List<String>();

        [Required]
        public Guid LeaderId { get; set; }

        [Required]
        public Guid OperationalObjectiveId { get; set; }

        public String Type { get; set; }

        public Guid? ParentId { get; set; }
    }

    public class CreateProjectCommandHandler : IRequestHandler<CreateTransitionCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CreateProjectCommandHandler(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CommandResponse> Handle(CreateTransitionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.StartedAt > request.Deadline)
                    return CommandResponse.Failure(400, "تاریخ شروع نمیتواند از تاریخ پایان کمتر باشد");

                if (!_context.People.Any(b => b.Id == request.LeaderId))
                    return CommandResponse.Failure(400, "راهبر انتخاب شده نامعتبر است");

                if (!_context.OperationalObjectives.Any(b => b.Id == request.OperationalObjectiveId))
                    return CommandResponse.Failure(400, "اقدام عملیاتی انتخاب شده نامعتبر است");

                var transition = _mapper.Map<Transition>(request);

                _context.Transitions.Add(transition);

                if (await _context.SaveChangesAsync(cancellationToken) > 0)
                    return CommandResponse.Success(transition.Id);

                return CommandResponse.Failure(400);
            }
            catch (Exception ex)
            {
                return CommandResponse.Failure(500, ex.Message);
            }

        }
    }
}
