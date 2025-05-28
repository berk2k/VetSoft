using TermProjectBackend.Models;
using TermProjectBackend.Models.Dto;

namespace TermProjectBackend.Source.Svc.Interfaces
{
    public interface IVetStaffService
    {
        public VetStaff CreateVetStaff(CreateNewStaffDTO vetStaffDTO);

        public int getStaffId(VetStaff vetStaff);

        public Task<LoginResponseVetStaffDTO> Login(LoginRequestVetStaffDTO loginReguestDTO);

        public void DeleteVetStaff(int id);

        public void UpdateVetStaff(UpdateVetStaffDTO vetStaffDTO);

        public List<VetStaff> GetAllStaff(int page, int pageSize);


    }
}
