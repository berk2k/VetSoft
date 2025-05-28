using TermProjectBackend.Models.Dto;
using TermProjectBackend.Models;

namespace TermProjectBackend.Source.Svc.Interfaces
{
    public interface IPetService
    {
        bool IsPetUnique(string name);
        public Pet AddPet(AddPetRequestDTO addPetRequestDTO/*,int id*/);

        public string GetPetNameById(int id);

        public List<GetPetDTO> GetPetInformationById(int id);
    }
}
