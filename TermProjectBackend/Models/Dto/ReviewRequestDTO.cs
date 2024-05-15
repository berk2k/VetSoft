using System.ComponentModel.DataAnnotations.Schema;

namespace TermProjectBackend.Models.Dto
{
    public class ReviewRequestDTO
    {
       
        public int userId { get; set; }

        
        public int petId { get; set; }

        public string message { get; set; }

        public DateTime? SentAt { get; set; }
    }
}
