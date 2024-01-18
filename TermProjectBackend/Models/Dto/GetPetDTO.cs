namespace TermProjectBackend.Models.Dto
{
    public class GetPetDTO
    {
        public int OwnerId { get; set; }

        public string Name { get; set; }
        public string Species { get; set; }

        public int Age { get; set; }

    }
}
