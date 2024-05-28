using TermProjectBackend.Models.Dto;
using TermProjectBackend.Models;

namespace TermProjectBackend.Source.Svc
{
    public interface IPetService
    {
        bool IsPetUnique(String name);
        public Pet AddPet(AddPetRequestDTO addPetRequestDTO/*,int id*/);

        public String GetPetNameById(int id);

        public List<GetPetDTO> GetPetInformationById(int id);
    }
}
