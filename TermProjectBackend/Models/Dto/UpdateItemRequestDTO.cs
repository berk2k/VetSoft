namespace TermProjectBackend.Models.Dto
{
    public class UpdateItemRequestDTO
    {
        public int id { get; set; }
        public string ItemName { get; set; }

        public int Count { get; set; }
    }
}
