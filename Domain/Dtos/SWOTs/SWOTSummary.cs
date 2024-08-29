namespace Domain.Dtos.SWOTs
{
    public class SWOTSummary
    {
        public Guid Id { get; set; }
        public String Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Type { get; set; }
    }
}
