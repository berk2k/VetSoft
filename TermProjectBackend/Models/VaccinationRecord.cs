using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TermProjectBackend.Models
{
    public class VaccinationRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int vaccinationId { get; set; }


        [ForeignKey("User")]
        public int userId { get; set; }

        [ForeignKey("Pet")]
        public int petId { get; set; }

        public string petName { get; set; }

        public string vaccine_name { get; set; }

        public string vaccine_date { get; set; }
    }
}
