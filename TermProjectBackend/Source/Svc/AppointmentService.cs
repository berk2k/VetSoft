using TermProjectBackend.Context;
using TermProjectBackend.Models;
using TermProjectBackend.Models.Dto;
using System.Text;
using RabbitMQ.Client;
using Microsoft.AspNetCore.Connections;

namespace TermProjectBackend.Source.Svc
{
    public class AppointmentService : IAppointmentService
    {
        private readonly VetDbContext _vetDb;
        private readonly INotificationService _notificationService;
        private readonly ConnectionFactory _connectionFactory;
        private const string QueueNameDelete = "delete_appointment_queue";
        private const string QueueNameUpdate = "update_appointment_queue";

        public AppointmentService(VetDbContext vetDb, INotificationService notificationService)
        {
            _vetDb = vetDb;
            _notificationService = notificationService;
            _connectionFactory = new ConnectionFactory
            {
                HostName = "localhost", // RabbitMQ sunucu adresi
                Port = 5672, // RabbitMQ varsayılan bağlantı noktası
                UserName = "guest", // RabbitMQ kullanıcı adı
                Password = "guest" // RabbitMQ şifre
            };

        }

        public Appointment BookAppointment(AppointmentDTO newAppointment, int id)
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
                PetName = newAppointment.PetName,
                Reasons = newAppointment.Reasons
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
            // Find the appointment in the database
            var existingAppointment = _vetDb.Appointments.FirstOrDefault(a => a.AppointmentId == id);

            if (existingAppointment != null)
            {
                // Remove the appointment from the database
                _vetDb.Appointments.Remove(existingAppointment);
                _vetDb.SaveChanges();
                //SendDeleteAppointmentMessageToRabbitMQ();
                var notificationRequest = new NotificationRequestDTO
                {
                    userId = existingAppointment.ClientID,
                    message = "Your appointment has been deleted"
                };
                _notificationService.Notification(notificationRequest);
            }
            else
            {
                // Handle the case where the appointment does not exist
                throw new InvalidOperationException("Appointment not found.");
            }
        }


        public void UpdateAppointment(ManageAppointmentDTO appointment)
        {
            try
            {
                var appointmentToUpdate = _vetDb.Appointments.FirstOrDefault(i => i.AppointmentId == appointment.Id);

                if (appointmentToUpdate != null)
                {
                    appointmentToUpdate.AppointmentDateTime = appointment.AppointmentDateTime;
                    _vetDb.SaveChanges();
                    SendUpdateAppointmentMessageToRabbitMQ(appointment.AppointmentDateTime);
                    var notificationRequest = new NotificationRequestDTO
                    {
                        userId = appointmentToUpdate.ClientID,
                        message = $"Your appointment has been updated to {appointment.AppointmentDateTime}"
                    };
                    _notificationService.Notification(notificationRequest);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new InvalidOperationException("An error occurred while updating the appointment.", ex);
            }
        }

        public List<Appointment> GetAppointmentsPerPage(int page, int pageSize)
        {
            return _vetDb.Appointments
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public List<Appointment> GetUserAppointments(int page, int pageSize, int userId)
        {
            return _vetDb.Appointments
                .Where(appointment => appointment.ClientID == userId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public List<Appointment> GetUserAppointmentsWOPagination(int userId)
        {
            return _vetDb.Appointments
                .Where(appointment => appointment.ClientID == userId)
                .ToList();
        }

        //private void SendDeleteAppointmentMessageToRabbitMQ()
        //{
        //    string deleteMessage = "Your appointment deleted";
        //    using (var connection = _connectionFactory.CreateConnection())
        //    using (var channel = connection.CreateModel())
        //    {

        //        channel.QueueDeclare(queue: QueueNameDelete,
        //                             durable: false,
        //                             exclusive: false,
        //                             autoDelete: false,
        //                             arguments: null);

        //        channel.ExchangeDeclare("direct_exchange", ExchangeType.Fanout, true);

        //        // Bildirim verisini JSON formatına dönüştür
        //        string message = Newtonsoft.Json.JsonConvert.SerializeObject(deleteMessage);
        //        var body = Encoding.UTF8.GetBytes(message);






        //        channel.BasicPublish(exchange: "direct_exchange",
        //                             routingKey: QueueNameDelete,
        //                             basicProperties: null,
        //                             body: body);
        //        channel.Close();
        //        connection.Close();
        //    }
        //}

        private void SendUpdateAppointmentMessageToRabbitMQ(DateTime newAppointmentDate)
        {
            string updateMsg = "Your appointment date updated. New date:";
            using (var connection = _connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {

                channel.QueueDeclare(queue: QueueNameUpdate,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                channel.ExchangeDeclare("direct_exchange", ExchangeType.Fanout, true);

                // Bildirim verisini JSON formatına dönüştür
                string message = Newtonsoft.Json.JsonConvert.SerializeObject(updateMsg + newAppointmentDate);
                var body = Encoding.UTF8.GetBytes(message);






                channel.BasicPublish(exchange: "direct_exchange",
                                     routingKey: QueueNameUpdate,
                                     basicProperties: null,
                                     body: body);
                channel.Close();
                connection.Close();
            }
        }

    }
}