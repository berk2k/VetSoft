using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TermProjectBackend.Models
{
    public class Pet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int OwnerID { get; set; }

        //public User User { get; set; }
        public string Name { get; set; }

        public string Species { get; set; }
        public string Breed { get; set; }
        public string Color { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public double Weight { get; set; }

        public string Allergies { get; set; }
    }
}
