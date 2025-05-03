using TermProjectBackend.Context;
using TermProjectBackend.Models;
using TermProjectBackend.Models.Dto;
using System.Text;

using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TermProjectBackend.Source.Svc
{
    public class AppointmentService : IAppointmentService
    {
        private readonly VetDbContext _vetDb;
        private readonly INotificationService _notificationService;
        private readonly ConnectionFactory _connectionFactory;
        private const string QueueNameDelete = "delete_appointment_queue";
        private const string QueueNameUpdate = "update_appointment_queue";
        private readonly RabbitMqService _rabbitMqService;



        private readonly ILogger<AppointmentService> _logger;

        public AppointmentService(VetDbContext vetDb, INotificationService notificationService, ILogger<AppointmentService> logger)
        {
            _vetDb = vetDb;
            _notificationService = notificationService;
            _logger = logger;
            _logger.LogInformation("AppointmentService initialized");
        }

        public Appointment BookAppointment(AppointmentDTO newAppointment, int id)
        {
            _logger.LogInformation("BookAppointment started for user ID: {UserId} at {AppointmentTime}",
                id, newAppointment.AppointmentDateTime);

            try
            {
            User user = _vetDb.Users.Find(id);

            if (user == null)
            {
                    _logger.LogWarning("BookAppointment failed: User with ID {UserId} not found", id);
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

                _logger.LogInformation("BookAppointment successful: Created appointment ID {AppointmentId} for user {UserName} (ID: {UserId})",
                    appointment.AppointmentId, user.Name, id);

            return appointment;
        }
            catch (Exception ex)
            {
                _logger.LogError(ex, "BookAppointment exception for user ID: {UserId}", id);
                throw;
            }
        }

        public Appointment GetAppointmentById(int appointmentId)
        {
            _logger.LogDebug("GetAppointmentById called with ID: {AppointmentId}", appointmentId);

            try
            {
                var appointment = _vetDb.Appointments.Find(appointmentId);

                if (appointment == null)
                {
                    _logger.LogWarning("GetAppointmentById: No appointment found with ID {AppointmentId}", appointmentId);
        }
                else
                {
                    _logger.LogDebug("GetAppointmentById successful for ID: {AppointmentId}", appointmentId);
                }

                return appointment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAppointmentById exception for appointment ID: {AppointmentId}", appointmentId);
                throw;
            }
        }

        public void RemoveAppointment(int id)
        {
            _logger.LogInformation("RemoveAppointment started for appointment ID: {AppointmentId}", id);

            try
            {
            // Find the appointment in the database
            var existingAppointment = _vetDb.Appointments.Find(id);

            if (existingAppointment != null)
            {
                    _logger.LogInformation("RemoveAppointment: Found appointment ID {AppointmentId} for client ID {ClientId}, name {ClientName}",
                        id, existingAppointment.ClientID, existingAppointment.ClientName);

                // Remove the appointment from the database
                _vetDb.Appointments.Remove(existingAppointment);
                _vetDb.SaveChanges();

                    _logger.LogInformation("RemoveAppointment: Successfully removed appointment ID {AppointmentId}", id);

                    // Send notification
                var notificationRequest = new NotificationRequestDTO
                {
                    userId = existingAppointment.ClientID,
                    message = "Your appointment has been deleted"
                };

                    _logger.LogDebug("RemoveAppointment: Sending notification to user {UserId}: {Message}",
                        notificationRequest.userId, notificationRequest.message);

                _notificationService.Notification(notificationRequest);

                    _logger.LogDebug("RemoveAppointment: Notification sent successfully");
            }
            else
            {
                    _logger.LogWarning("RemoveAppointment failed: Appointment with ID {AppointmentId} not found", id);
                throw new InvalidOperationException("Appointment not found.");
            }
        }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RemoveAppointment exception for appointment ID: {AppointmentId}", id);
                throw;
            }
        }


        public void UpdateAppointment(ManageAppointmentDTO appointment)
        {
            _logger.LogInformation("UpdateAppointment started for appointment ID: {AppointmentId}", appointment.Id);

            try
            {
                var appointmentToUpdate = _vetDb.Appointments.Find(appointment.Id);

                if (appointmentToUpdate == null)
                {
                    _logger.LogWarning("UpdateAppointment failed: No appointment found with ID {AppointmentId}", appointment.Id);
                    throw new InvalidOperationException($"No appointment found with ID {appointment.Id}");
                }

                _logger.LogInformation("UpdateAppointment: Updating appointment ID {AppointmentId} for client {ClientName} (ID: {ClientId})",
                    appointment.Id, appointmentToUpdate.ClientName, appointmentToUpdate.ClientID);

                DateTime oldDateTime = appointmentToUpdate.AppointmentDateTime;
                appointmentToUpdate.AppointmentDateTime = appointment.AppointmentDateTime;
                _vetDb.SaveChanges();

                _logger.LogInformation("UpdateAppointment: Successfully updated appointment ID {AppointmentId} from {OldDateTime} to {NewDateTime}",
                    appointment.Id, oldDateTime, appointment.AppointmentDateTime);

                var notificationRequest = new NotificationRequestDTO
                {
                    userId = appointmentToUpdate.ClientID,
                    message = $"Your appointment has been updated to {appointment.AppointmentDateTime}"
                };

                _logger.LogDebug("UpdateAppointment: Preparing notification for user {UserId}: {Message}",
                    notificationRequest.userId, notificationRequest.message);

                string serializedMessage = Newtonsoft.Json.JsonConvert.SerializeObject(notificationRequest);
                _logger.LogTrace("UpdateAppointment: Serialized notification message: {SerializedMessage}", serializedMessage);

                _logger.LogDebug("UpdateAppointment: Sending notification to user {UserId}", notificationRequest.userId);
                _notificationService.Notification(notificationRequest);
                _logger.LogDebug("UpdateAppointment: Notification sent successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateAppointment exception for appointment ID: {AppointmentId}", appointment.Id);
                throw new InvalidOperationException("An error occurred while updating the appointment.", ex);
            }
        }



        public List<Appointment> GetAppointmentsPerPage(int page, int pageSize)
        {
            _logger.LogInformation("GetAppointmentsPerPage called with page: {Page}, pageSize: {PageSize}", page, pageSize);

            try
            {
                var appointments = _vetDb.Appointments
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsQueryable()
                .ToList();

                _logger.LogDebug("GetAppointmentsPerPage: Retrieved {Count} appointments", appointments.Count);
                return appointments;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAppointmentsPerPage exception with page: {Page}, pageSize: {PageSize}", page, pageSize);
                throw;
            }
        }

        public List<Appointment> GetUserAppointments(int page, int pageSize, int userId)
        {
            _logger.LogInformation("GetUserAppointments called for user ID: {UserId}, page: {Page}, pageSize: {PageSize}",
                userId, page, pageSize);

            try
            {
                var appointments = _vetDb.Appointments
                .Where(appointment => appointment.ClientID == userId)
                .AsQueryable()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

                _logger.LogDebug("GetUserAppointments: Retrieved {Count} appointments for user ID: {UserId}",
                    appointments.Count, userId);

                return appointments;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetUserAppointments exception for user ID: {UserId}, page: {Page}, pageSize: {PageSize}",
                    userId, page, pageSize);
                throw;
            }
        }

        public List<Appointment> GetUserAppointmentsWOPagination(int userId)
        {
            _logger.LogInformation("GetUserAppointmentsWOPagination called for user ID: {UserId}", userId);

            try
            {
                var appointments = _vetDb.Appointments
                .Where(a => a.ClientID == userId)
                .AsQueryable()
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






                _logger.LogDebug("GetUserAppointmentsWOPagination: Retrieved {Count} appointments for user ID: {UserId}",
                    appointments.Count, userId);

                return appointments;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetUserAppointmentsWOPagination exception for user ID: {UserId}", userId);
                throw;
            }
        }

        
    }
}