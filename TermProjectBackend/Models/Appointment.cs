using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TermProjectBackend.Models
{
    public class Appointment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AppointmentId { get; set; }

        [ForeignKey("User")]
        public int ClientID { get; set; }

        public DateTime AppointmentDateTime { get; set; }
        //public User User { get; set; }
        public string ClientName { get; set; }

        public string PetName { get; set; }

        public string Reasons { get; set; }
    }
}
