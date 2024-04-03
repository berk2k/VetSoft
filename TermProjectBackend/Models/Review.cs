using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TermProjectBackend.Models
{
    public class Review
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int reviewId { get; set; }


        [ForeignKey("User")]
        public int userId { get; set; }

        [ForeignKey("Pet")]
        public int petId { get; set; }

        public string message { get; set; }

        public string userName { get; set; }

        public string petName { get; set; }
    }
}
