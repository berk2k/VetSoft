using TermProjectBackend.Context;
using TermProjectBackend.Models;
using TermProjectBackend.Models.Dto;

namespace TermProjectBackend.Source.Svc
{
    public class PetService : IPetService
    {
        private readonly VetDbContext _vetDb;
        

        public PetService(VetDbContext vetDb)
        {
            _vetDb = vetDb;
            
        }

        public List<GetPetDTO> GetPetInformationById(int ownerId)
        {

            List<GetPetDTO> petDTOs = _vetDb.Pets
                .Where(p => p.OwnerID == ownerId)
                .Select(p => new GetPetDTO
                {
                    PetId = p.Id,
                    Name = p.Name,
                    Species = p.Species,
                    Age = p.Age
                })
                .ToList();


            return petDTOs;
        }


        public Pet AddPet(AddPetRequestDTO addPetRequestDTO/*,int id*/)
        {
            Pet pet = new Pet()
            {
                //OwnerID = id,
                OwnerID = addPetRequestDTO.id,
                Name = addPetRequestDTO.Name,
                Species = addPetRequestDTO.Species,
                Breed = addPetRequestDTO.Breed,
                Color = addPetRequestDTO.Color,
                Age = addPetRequestDTO.Age,
                Gender = addPetRequestDTO.Gender,
                Weight = addPetRequestDTO.Weight,
                Allergies = addPetRequestDTO.Allergies
            };

            _vetDb.Pets.Add(pet);
            _vetDb.SaveChanges();

            return pet;
        }

        public bool IsPetUnique(String name)
        {
            var pet = _vetDb.Pets.FirstOrDefault(p => p.Name == name);

            if (pet == null)
            {
                return true;
            }

            return false;
        }

        
    }
}

