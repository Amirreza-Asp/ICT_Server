namespace Domain.Dtos.BigGoals
{
    public class BigGoalSelectList
    {
        public Guid Id { get; set; }
        public String Title { get; set; }
        public int OperationalObjectiveCount { get; set; }
        public int IndicatorsCount { get; set; }
    }
}
