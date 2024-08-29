using AutoMapper;
using Domain.Dtos.OperationalObjectives;
using Domain.Dtos.Transitions;
using Domain.Entities.Business;
using Domain.Utiltiy;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CQRS.Business.OperationalObjectives
{
    public class OperationalObjectivesByBigGoalIdQuery : IRequest<List<OperationalObjectiveCard>>
    {
        [Required]
        public Guid BigGoalId { get; set; }
    }

    public class OperationalObjectivesByBigGoalIdQueryHandler : IRequestHandler<OperationalObjectivesByBigGoalIdQuery, List<OperationalObjectiveCard>>
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public OperationalObjectivesByBigGoalIdQueryHandler(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<OperationalObjectiveCard>> Handle(OperationalObjectivesByBigGoalIdQuery request, CancellationToken cancellationToken)
        {
            var data =
                await _context.OperationalObjectives
                    .Where(b => b.BigGoalId == request.BigGoalId)
                    .Include(b => b.Indicators)
                        .ThenInclude(b => b.Indicator)
                            .ThenInclude(b => b.Progresses)
                    .ToListAsync(cancellationToken);

            var opCards =
                _mapper.Map<List<OperationalObjectiveCard>>(data);



            for (int i = 0; i < opCards.Count; i++)
            {
                opCards[i].Progress = data[i].Progress;

                var sumRealProgress = data[i].Indicators.Sum(
                                e => (!e.Indicator.Progresses.Any() ? 0 : e.Indicator.Progresses.OrderByDescending(b => b.ProgressTime).First().Value - e.Indicator.InitValue) * 100 / (e.Indicator.GoalValue - e.Indicator.InitValue));
                opCards[i].RealProgress = data[i].Indicators.Any() ? (int)sumRealProgress / data[i].Indicators.Count() : 0;

                var transitions = GetTransitions(data[i].Id);

                var indicators =
                    await _context.TransitionIndicators
                        .Include(b => b.Indicator)
                        .ThenInclude(b => b.Progresses)
                        .Where(b => transitions.Select(b => b.Id).Contains(b.TransitionId))
                        .ToListAsync();

                var nestedTransitions = new List<NestedTransition>();

                foreach (var item in transitions.Where(b => b.ParentId == null))
                    nestedTransitions.Add(ConvertToNested(item.Id, transitions, indicators));

                opCards[i].Transitions = nestedTransitions;
            };

            return opCards;
        }

        private NestedTransition ConvertToNested(Guid id, List<Transition> transitions, List<TransitionIndicator> transitionIndicators)
        {
            var transition = transitions.First(b => b.Id == id);
            var indicators = transitionIndicators.Where(b => b.TransitionId == id).Select(b => b.Indicator).ToList();

            var nestedTransition = new NestedTransition
            {
                Id = transition.Id,
                Deadline = transition.Deadline,
                Active = transition.Deadline > DateTime.Now && transition.StartedAt < DateTime.Now,
                Financials = transition.Financials.Select(b => b.Title).ToList(),
                Title = transition.Title,
                Type = transition.Type == TransitionType.Project ? "پروژه" : "اقدام",
                IndicatorsCount = indicators.Count(),
                Progress = Calculator.CalcProgress(indicators),
                ParentId = transition.ParentId,
            };

            var sumRealProgress = 0;
            var realProgressCount = 0;

            foreach (var indicator in indicators)
            {
                realProgressCount += 1;

                if (indicator.Progresses.Any())
                {
                    var lastProgress = indicator.Progresses.OrderByDescending(b => b.ProgressTime).First();
                    sumRealProgress += Convert.ToInt32((lastProgress.Value - indicator.InitValue) * 100 / (indicator.GoalValue - indicator.InitValue));
                }
            }
            nestedTransition.RealProgress = realProgressCount == 0 ? 0 : sumRealProgress / realProgressCount;



            nestedTransition.Childs = new List<NestedTransition>();
            var childs = transitions.Where(b => b.ParentId == id).ToList();

            foreach (var child in childs)
            {
                var nestedChild = ConvertToNested(child.Id, transitions, transitionIndicators);
                nestedTransition.Childs.Add(nestedChild);
            }

            return nestedTransition;
        }

        List<Transition> GetTransitions(Guid opId)
        {
            var data = _context.Transitions.Where(b => b.OperationalObjectiveId == opId).Include(b => b.Financials).ToList();
            return data;
        }

    }

    public class Test
    {
        public Guid TId { get; set; }
        public String FTitle { get; set; }
    }
}
