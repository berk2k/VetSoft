namespace TermProjectBackend.Models.Dto
{
    public class GetPetDTO
    {
        public int PetId { get; set; }

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
