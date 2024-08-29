using AutoMapper;
using Domain.Dtos.BigGoals;
using Domain.Dtos.Companies;
using Domain.Dtos.OperationalObjectives;
using Domain.Dtos.People;
using Domain.Dtos.Programs;
using Domain.Dtos.Refrences;
using Domain.Dtos.Shared;
using Domain.Dtos.Strategies;
using Domain.Dtos.SWOTs;
using Domain.Dtos.Transitions;
using Domain.Entities.Account;
using Domain.Entities.Business;
using Domain.SubEntities;
using Infrastructure.CQRS.Business.BigGoals;
using Infrastructure.CQRS.Business.HardwareEquipments;
using Infrastructure.CQRS.Business.OperationalObjectives;
using Infrastructure.CQRS.Business.People;
using Infrastructure.CQRS.Business.Systems;
using Infrastructure.CQRS.Business.Transitions;

namespace Infrastructure.Profiles
{
    public class BusinessProfile : Profile
    {

        public BusinessProfile()
        {
            #region BigGoal
            CreateMap<BigGoal, BigGoal>();

            CreateMap<BigGoal, BigGoalSelectList>()
                .ForMember(b => b.OperationalObjectiveCount, d => d.MapFrom(d => d.OperationalObjectives.Count()))
                .ForMember(b => b.IndicatorsCount, d => d.MapFrom(d => d.Indicators.Count()));

            CreateMap<BigGoal, BigGoalSummary>()
                .ForMember(b => b.OperationalObjectiveCount, d => d.MapFrom(d => d.OperationalObjectives.Count()))
                .ForMember(b => b.IndicatorsCount, d => d.MapFrom(d => d.Indicators.Count()))
                .ForMember(b => b.Progress, d => d.MapFrom(e => e.Progress));

            CreateMap<BigGoal, BigGoalsListDto>()
                .ForMember(b => b.Year, d => d.MapFrom(e => e.Programs.First().Program.StartedAt.Year.ToString()));

            CreateMap<CreateBigGoalCommand, BigGoal>()
               .ForMember(b => b.Id, d => d.MapFrom(e => Guid.NewGuid()));

            CreateMap<BigGoal, BigGoalDetails>()
                .ForMember(b => b.ProgramYear, d => d.MapFrom(e => e.Programs.First().Program.StartedAt.Year.ToString()))
                .ForMember(b => b.Progress, d => d.Ignore())
                .ForMember(b => b.OperationalObjectiveCount, d => d.MapFrom(e => e.OperationalObjectives.Count()))
                .ForMember(b => b.Indicators, d => d.MapFrom(e => e.Indicators.Select(e => e.Indicator)));
            #endregion

            #region OperationalObjective
            CreateMap<CreateOperationalObjectiveCommand, OperationalObjective>()
                .ForMember(b => b.Id, d => d.MapFrom(e => Guid.NewGuid()))
                .ForMember(b => b.Budget, d => d.MapFrom(e => -1));
            CreateMap<OperationalObjective, OperationalObjectiveSummary>()
                .ForMember(b => b.ProjectsCount, d =>
                    d.MapFrom(e => e.Transitions.Where(b => b.Type == TransitionType.Project && b.ParentId == null).Count()))
                .ForMember(b => b.ActionsCount, d =>
                    d.MapFrom(e => e.Transitions.Where(b => b.Type == TransitionType.Action && b.ParentId == null).Count()))
                .ForMember(b => b.IndicatorsCount, d => d.MapFrom(e => e.Indicators.Count));

            CreateMap<OperationalObjective, OperationalObjectiveCard>()
                .ForMember(b => b.Active, d => d.MapFrom(b => b.Deadline >= DateTime.Now))
                .ForMember(b => b.IndicatorsCount, d => d.MapFrom(b => b.Indicators.Count()))
                .ForMember(b => b.Progress, d => d.Ignore());

            CreateMap<OperationalObjectiveIndicator, OperationalObjectiveIndicator>();
            CreateMap<OperationalObjective, OperationalObjectiveDetails>()
              .ForMember(b => b.ProjectsCount, d => d.MapFrom(b => b.Transitions.Where(b => b.Type == TransitionType.Project).Count()))
              .ForMember(b => b.Active, d => d.MapFrom(b => b.Deadline >= DateTime.Now))
              .ForMember(b => b.Title, d => d.MapFrom(e => e.BigGoal.Title + " > " + e.Title))
              .ForMember(b => b.PracticalActionsCount, d => d.MapFrom(b => b.Transitions.Where(b => b.Type == TransitionType.Action).Count()))
              .ForMember(b => b.Indicators, d => d.MapFrom(b => b.Indicators.Select(e => e.Indicator)))
              .ForMember(b => b.Progress, d => d.Ignore());
            #endregion

            #region Person
            CreateMap<Person, PersonSummary>()
                .ForMember(b => b.Expertises, d => d.MapFrom(b => b.Expertises.Select(d => d.Title)));
            CreateMap<Person, SelectSummary>()
                .ForMember(b => b.Title, d => d.MapFrom(e => e.Name + " " + e.Family));
            CreateMap<CreatePersonCommand, Person>()
                .ForMember(b => b.Expertises, d => d.MapFrom(b => b.Expertises.Select(d => new Expertise { Title = d })))
                .ForMember(b => b.Id, d => d.MapFrom(d => Guid.NewGuid()));
            #endregion

            #region HardwareEquipment
            // Hardware equipment
            CreateMap<HardwareEquipment, HardwareEquipment>();
            CreateMap<HardwareEquipment, SelectSummary>();
            CreateMap<CreateHardwareEquipmentCommand, HardwareEquipment>()
                .ForMember(b => b.Id, d => d.MapFrom(r => Guid.NewGuid()));
            #endregion

            #region System
            CreateMap<Domain.Entities.Business.System, Domain.Entities.Business.System>();
            CreateMap<Domain.Entities.Business.System, SelectSummary>();
            CreateMap<Domain.Entities.Business.System, SystemDetails>();
            CreateMap<CreateSystemCommand, Domain.Entities.Business.System>()
                .ForMember(b => b.Id, d => d.MapFrom(r => Guid.NewGuid()))
                .ForMember(b => b.BuildInCompany, d => d.MapFrom(e => e.Company))
                .ForMember(b => b.Company, d => d.Ignore());
            #endregion

            #region Transition
            CreateMap<CreateTransitionCommand, Transition>()
               .ForMember(b => b.Financials, d => d.MapFrom(b => b.Financials.Select(d => new Financial { Title = d })))
               .ForMember(b => b.Type, d => d.MapFrom(e => e.Type == "پروژه" ? TransitionType.Project : TransitionType.Action))
               .ForMember(b => b.Id, d => d.MapFrom(d => Guid.NewGuid()));

            CreateMap<Transition, TransitionCard>()
                .ForMember(b => b.Active, b => b.MapFrom(d => d.Deadline >= DateTime.Now))
                .ForMember(b => b.Type, b => b.MapFrom(d => d.Type == TransitionType.Project ? "پروژه" : "اقدام"))
                .ForMember(b => b.Financials, d => d.MapFrom(b => b.Financials.Select(s => s.Title)))
                .ForMember(b => b.Deadline, d => d.MapFrom(b => b.Deadline))
                .ForMember(b => b.IndicatorsCount, d => d.MapFrom(e => e.Indicators.Count()));

            CreateMap<Transition, TransitionDetails>()
               .ForMember(b => b.Financials, d => d.MapFrom(b => b.Financials.Select(d => d.Title)))
               .ForMember(b => b.Type, d => d.MapFrom(b => b.Type == TransitionType.Project ? "پروژه" : "اقدام"))
               .ForMember(b => b.LeaderName, d => d.MapFrom(d => String.Concat(d.Leader.Name, ' ', d.Leader.Family)))
               .ForMember(b => b.Indicators, d => d.MapFrom(d => d.Indicators.Select(e => e.Indicator)))
               .ForMember(b => b.Title, d => d.MapFrom(e => e.OperationalObjective.BigGoal.Title + " > " + e.OperationalObjective.Title + " > " + e.Title))
               .ForMember(b => b.OperationalObjectiveTitle, d => d.MapFrom(d => d.Title));

            CreateMap<TransitionIndicator, IndicatorDetails>()
                .ForAllMembers(e => e.MapFrom(d => d.Indicator));

            CreateMap<Transition, TransitionSummary>()
                .ForMember(b => b.ProjectsCount, d => d.MapFrom(e => e.Childs.Where(b => b.Type == TransitionType.Project).Count()))
                .ForMember(b => b.ActionsCount, d => d.MapFrom(e => e.Childs.Where(b => b.Type == TransitionType.Action).Count()))
                .ForMember(b => b.IndicatorsCount, d => d.MapFrom(e => e.Indicators.Count));

            CreateMap<Transition, Transition>();
            #endregion

            #region Company
            CreateMap<Company, CompanyBigGoals>()
                .ForMember(b => b.CompanyName, d => d.MapFrom(e => e.Title))
                .ForMember(b => b.CompanyId, d => d.MapFrom(e => e.Id))
                .ForMember(b => b.BigGoals, d => d.MapFrom(e => e.BigGoals));


            CreateMap<Company, CompanySummary>()
                .ForMember(b => b.Title, d => d.MapFrom(e => e.Title))
                .ForMember(b => b.Province, d => d.MapFrom(e => e.Province))
                .ForMember(b => b.City, d => d.MapFrom(e => e.City));

            CreateMap<Company, SelectSummary>();
            #endregion

            #region Indicators
            CreateMap<Indicator, IndicatorCard>()
                .ForMember(b => b.Title, e => e.MapFrom(e => e.Category.Title))
                .ForMember(b => b.RealCurrentValue, e => e.MapFrom(e =>
                     e.Progresses.Any() ?
                        e.Progresses.OrderByDescending(b => b.ProgressTime).First().Value : 0))
                .ForMember(b => b.RealProgress, e => e.MapFrom(e =>
                     e.Progresses.Any() ?
                        Convert.ToInt32((e.Progresses.OrderByDescending(b => b.ProgressTime).First().Value - e.InitValue) * 100 /
                        (e.GoalValue - e.InitValue)) : 0));

            CreateMap<Indicator, IndicatorDetails>()
                  .ForMember(b => b.Title, e => e.MapFrom(e => e.Category.Title))
                .ForMember(b => b.RealCurrentValue, e => e.MapFrom(e =>
                     e.Progresses.Any() ?
                        e.Progresses.OrderByDescending(b => b.ProgressTime).First().Value : 0))
                .ForMember(b => b.RealProgress, e => e.MapFrom(e =>
                     e.Progresses.Any() ?
                        Convert.ToInt32((e.Progresses.OrderByDescending(b => b.ProgressTime).First().Value - e.InitValue) * 100 /
                        (e.GoalValue - e.InitValue)) : 0))
                .ForMember(b => b.PeriodDisplay, d => d.MapFrom(e => e.Period.ToString()));
            #endregion

            #region Program
            CreateMap<Domain.Entities.Business.Program, ProgramSummary>()
                .ForMember(b => b.BigGoalsCount, b => b.MapFrom(e => e.BigGoals.Count()));
            CreateMap<Domain.Entities.Business.Program, ProgramDetails>();
            #endregion

            #region Perspective
            CreateMap<Perspective, PerspectiveSummary>();
            #endregion

            #region SWOT
            CreateMap<SWOT, SWOTSummary>()
                .ForMember(b => b.Type, d => d.MapFrom(e => Convert.ToInt32(e.Type)));
            #endregion

            #region Strategy
            CreateMap<Strategy, StrategySummary>();
            #endregion
        }
    }
}
