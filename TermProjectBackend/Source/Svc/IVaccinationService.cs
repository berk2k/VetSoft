using TermProjectBackend.Models;
using TermProjectBackend.Models.Dto;

namespace TermProjectBackend.Source.Svc
{
    public interface IVaccinationService
    {
        public VaccinationRecord AddVaccinationRecord(AddVaccinationRecordRequestDTO request);

        public void RemoveVaccinationReport(int id);

        public List<GetVaccinationReportDTO> GetAllVaccinationHistoryForUser(int id);

    }
}
