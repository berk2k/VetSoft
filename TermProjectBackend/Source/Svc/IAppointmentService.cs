using TermProjectBackend.Models;

namespace TermProjectBackend.Source.Svc
{
    public interface IAppointmentService
    {
        public Appointment BookAppointment(AppointmentDTO newAppointment,int id);

        Appointment GetAppointmentById(int appointmentId);
        void RemoveAppointment(Appointment appointment);

        
    }
}
