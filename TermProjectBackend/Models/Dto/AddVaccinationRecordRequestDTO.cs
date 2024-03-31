using System.ComponentModel.DataAnnotations.Schema;

namespace TermProjectBackend.Models.Dto
{
    public class AddVaccinationRecordRequestDTO
    {
        
        public int userId { get; set; }

        public int petId { get; set; }

        public string petName { get; set; }

        public string vaccine_name { get; set; }

        public string vaccine_date { get; set; }
    }
}
