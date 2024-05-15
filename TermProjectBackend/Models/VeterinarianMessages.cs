using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TermProjectBackend.Models
{
    public class VeterinarianMessages
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MessageId { get; set; }


        [ForeignKey("User")]
        public int UserId { get; set; }

        public string MessageTitle { get; set; }

        public string MessageText { get; set; }
        public string UserName { get; set; }

        public DateTime SentAt { get; set; }
    }
}
