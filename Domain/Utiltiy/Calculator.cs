using Domain.Dtos.Shared;
using Domain.Entities.Business;

namespace Domain.Utiltiy
{
    public class Calculator
    {

        public static int CalcProgress(IndicatorCard inc)
        {
            if (inc.FromDate > DateTime.Now)
                return 0;

            if (inc.Period == IndicatorPeriod.Day)
            {
                var date = inc.ToDate - inc.FromDate;
                double progress = (DateTime.Now - inc.FromDate).TotalDays / date.TotalDays;
                return Convert.ToInt32(progress * 100);
            }
            else if (inc.Period == IndicatorPeriod.Week)
            {
                var date = inc.ToDate - inc.FromDate;
                double progress = (DateTime.Now - inc.FromDate).TotalDays / 7 / (date.TotalDays / 7);
                return Convert.ToInt32(progress * 100);
            }
            else if (inc.Period == IndicatorPeriod.Month)
            {
                var date = inc.ToDate - inc.FromDate;
                double progress = (DateTime.Now - inc.FromDate).TotalDays / 30 / (date.TotalDays / 30);
                return Convert.ToInt32(progress * 100);
            }
            else if (inc.Period == IndicatorPeriod.Season)
            {
                var date = inc.ToDate - inc.FromDate;
                double progress = (DateTime.Now - inc.FromDate).TotalDays / 90 / (date.TotalDays / 90);
                return Convert.ToInt32(progress * 100);
            }
            else if (inc.Period == IndicatorPeriod.HalfYear)
            {
                var date = inc.ToDate - inc.FromDate;
                double progress = (DateTime.Now - inc.FromDate).TotalDays / 182 / (date.TotalDays / 182);
                return Convert.ToInt32(progress * 100);
            }
            else
            {
                var date = inc.ToDate - inc.FromDate;
                double progress = (DateTime.Now - inc.FromDate).TotalDays / 365 / (date.TotalDays / 365);
                return Convert.ToInt32(progress * 100);
            }
        }

        public static int CalcProgress(Indicator inc)
        {
            var incCard = new IndicatorCard { FromDate = inc.FromDate, Period = inc.Period, ToDate = inc.ToDate };
            return CalcProgress(incCard);
        }

        public static long CalcCurrentValue(IndicatorCard inc)
        {
            var progress = inc.ScheduleProgress;
            return (inc.GoalValue - inc.InitValue) / 100 * progress;
        }

        public static int CalcProgress(IEnumerable<Indicator> incs)
        {
            int total = 0;
            foreach (var inc in incs)
                total += CalcProgress(inc);

            return total / Math.Max(incs.Count(), 1);
        }

        public static int CalcProgress(IEnumerable<IndicatorCard> incs)
        {
            int total = 0;
            foreach (var inc in incs)
                total += CalcProgress(inc);

            return total / Math.Max(incs.Count(), 1);
        }

    }
}
