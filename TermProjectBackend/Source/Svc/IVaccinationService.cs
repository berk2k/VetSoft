using TermProjectBackend.Models;
using TermProjectBackend.Models.Dto;

namespace TermProjectBackend.Source.Svc
{
    public interface IVaccinationService
    {
        public VaccinationRecord AddVaccinationRecord(AddVaccinationRecordRequestDTO request);

        public void RemoveVaccinationReport(int id);

        public List<GetVaccinationReportDTO> GetAllVaccinationHistoryForUserWOPagination(int id);

        public List<GetVaccinationReportDTO> GetAllVaccinationHistoryForUser(int page, int pageSize, int id);

    }
}
