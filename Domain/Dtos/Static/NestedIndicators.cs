namespace Domain.Dtos.Static
{
    public class NestedIndicators
    {
        public Guid Id { get; set; }
        public String Title { get; set; }
        public List<NestedIndicators> Childs { get; set; } = new List<NestedIndicators>();
    }
}
