namespace Domain.Dtos.Transitions
{
    public class NestedTransition
    {
        public Guid Id { get; set; }
        public String Title { get; set; }

        public DateTime Deadline { get; set; }

        public bool Active { get; set; }

        public String Type { get; set; }

        public int Progress { get; set; }

        public int RealProgress { get; set; }

        public List<String> Financials { get; set; } = new List<String>();

        public int IndicatorsCount { get; set; }

        public Guid? ParentId { get; set; }

        public List<NestedTransition> Childs { get; set; }
    }
}
