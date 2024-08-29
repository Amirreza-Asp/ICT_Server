namespace Domain.Dtos.Shared
{
    public class MultiDataSelect
    {
        public String Title { get; set; }
        public List<MultiDataSelectItem> Data { get; set; }
    }

    public class MultiDataSelectItem
    {
        public dynamic Value { get; set; }
        public String Text { get; set; }
    }
}
