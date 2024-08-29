using Domain.Entities.Business;

namespace Domain.Dtos.Shared
{
    public class IndicatorCard
    {
        public Guid Id { get; set; }
        public String Title { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public long GoalValue { get; set; }
        public long InitValue { get; set; }

        public long ScheduleCurrentValue { get; set; }
        public int ScheduleProgress { get; set; }

        public long RealCurrentValue { get; set; }
        public int RealProgress { get; set; }

        public IndicatorPeriod Period { get; set; }
    }
}
