namespace TermProjectBackend.Models.Dto
{
    public class VetMessageDTO
    {
        public int userId { get; set; }

        public string messageText { get; set; }

        public string messageTitle { get; set; }

        public DateTime SentAt { get; set; }
    }
}
