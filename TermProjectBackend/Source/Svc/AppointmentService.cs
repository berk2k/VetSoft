using TermProjectBackend.Context;
using TermProjectBackend.Models;

namespace TermProjectBackend.Source.Svc
{
    public class AppointmentService : IAppointmentService
    {
        private readonly VetDbContext _vetDb;


        public AppointmentService(VetDbContext vetDb)
        {
            _vetDb = vetDb;

        }

        public Appointment BookAppointment(AppointmentDTO newAppointment,int id)
        {


            User user = _vetDb.Users.FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                // Handle the case where the user is not found
                throw new InvalidOperationException($"User with ID {id} not found.");
            }

            Appointment appointment = new Appointment()
            {
                ClientID = id,
                AppointmentDateTime = newAppointment.AppointmentDateTime,
                ClientName = user.Name,
                
            };

            _vetDb.Appointments.Add(appointment);
            _vetDb.SaveChanges();

            return appointment;
        }

        public Appointment GetAppointmentById(int appointmentId)
        {
            // Retrieve the appointment from the database based on the provided appointmentId
            return _vetDb.Appointments.FirstOrDefault(a => a.AppointmentId == appointmentId);
        }

        public void RemoveAppointment(Appointment appointment)
        {
            // Check if the appointment exists in the database
            var existingAppointment = _vetDb.Appointments.FirstOrDefault(a => a.AppointmentId == appointment.AppointmentId);

            if (existingAppointment != null)
            {
                // Remove the appointment from the database
                _vetDb.Appointments.Remove(existingAppointment);
                _vetDb.SaveChanges();
            }
            else
            {
                // Handle the case where the appointment does not exist
                throw new InvalidOperationException("Appointment not found.");
            }
        }

        

    }
}
