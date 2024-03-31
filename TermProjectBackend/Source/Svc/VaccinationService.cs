using TermProjectBackend.Context;
using TermProjectBackend.Models;
using TermProjectBackend.Models.Dto;

namespace TermProjectBackend.Source.Svc
{
    public class VaccinationService : IVaccinationService
    {
        private readonly VetDbContext _vetDb;
        public VaccinationService(VetDbContext vetDb)
        {
            _vetDb = vetDb;

        }

        public VaccinationRecord AddVaccinationRecord(AddVaccinationRecordRequestDTO request)
        {
            

            if (!_vetDb.Pets.Any(p => p.Id == request.petId && p.OwnerID == request.userId))
            {
                throw new InvalidOperationException("pet or user not found.");
            }

            
            var newRecord = new VaccinationRecord
            {

                    userId = request.userId,
                    petId = request.petId,
                    petName = request.petName,
                    vaccine_name = request.vaccine_name,
                    vaccine_date = request.vaccine_date
             
            };


                _vetDb.VaccinationRecord.Add(newRecord);
                _vetDb.SaveChanges();


                return newRecord;
            }

            public List<GetVaccinationReportDTO> GetAllVaccinationHistoryForUser(int id)
            {
                var vaccinationHistory = _vetDb.VaccinationRecord
                .Where(v => v.userId == id)
                .Select(v => new GetVaccinationReportDTO
                {
                    petName = v.petName,
                    vaccineName = v.vaccine_name,
                    date = v.vaccine_date
                    //date = DateTime.Parse(v.date).ToString("yyyy-MM-dd") // Parse string to DateTime
                })
                .ToList();

                return vaccinationHistory;
            }

            public void RemoveVaccinationReport(int id)
            {
                VaccinationRecord record = _vetDb.VaccinationRecord.Find(id);

                if (record == null)
                {

                    throw new InvalidOperationException($"Report with ID {id} not found.");
                }

                _vetDb.VaccinationRecord.Remove(record);

                _vetDb.SaveChanges();
            }
     }

}


