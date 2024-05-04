using TermProjectBackend.Models;
using TermProjectBackend.Models.Dto;

namespace TermProjectBackend.Source.Svc
{
    public interface IAppointmentService
    {
        public Appointment BookAppointment(AppointmentDTO newAppointment,int id);

        public Appointment GetAppointmentById(int appointmentId);
        public void RemoveAppointment(int id);

        public void UpdateAppointment(ManageAppointmentDTO appointment);

        List<Appointment> GetAppointmentsPerPage(int page, int pageSize);




    }
}
