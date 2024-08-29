namespace Domain.Dtos.Shared
{
    public class ListActionResult<T> where T : class
    {
        public List<T> Data { get; set; }
        public int Total { get; set; } = 0;
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 10;
        public int TotalPages => Size == 0 ? 0 : (int)Math.Ceiling((double)Total / Size);
    }
}
