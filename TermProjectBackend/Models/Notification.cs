using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TermProjectBackend.Models
{
    public class Notification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int notificationId { get; set; }


        [ForeignKey("User")]
        public int userId { get; set; }

        public string message { get; set; }

        public string userName { get; set; }

        public DateTime SentAt { get; set; }
    }
}
