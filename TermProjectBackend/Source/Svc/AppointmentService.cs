using TermProjectBackend.Context;
using TermProjectBackend.Models;
using TermProjectBackend.Models.Dto;

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

        public void RemoveAppointment(int id)
        {
            // Check if the appointment exists in the database
            var existingAppointment = _vetDb.Appointments.FirstOrDefault(a => a.AppointmentId == id);

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

        public void UpdateAppointment(ManageAppointmentDTO appointment)
        {
            var appointmentToUpdate = _vetDb.Appointments.FirstOrDefault(i => i.AppointmentId == appointment.Id);

            if (appointmentToUpdate != null)
            {
                
                appointmentToUpdate.AppointmentDateTime = appointment.AppointmentDateTime;

                _vetDb.SaveChanges();
            }
        }

        public List<Appointment> GetAppointmentsPerPage(int page, int pageSize)
        {
            return _vetDb.Appointments
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }
    }
}
