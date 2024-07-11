using Moq;
using MockQueryable.Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TermProjectBackend.Context;
using TermProjectBackend.Models;
using TermProjectBackend.Models.Dto;
using TermProjectBackend.Source.Svc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework.Legacy;

namespace TermProjectTests.Tests.ServiceTests
{
    [TestFixture]
    public class AppointmentServiceTests
    {
        private Mock<VetDbContext> _mockVetDbContext;
        private Mock<DbSet<Appointment>> _mockAppointmentSet;
        private Mock<DbSet<User>> _mockUserSet;
        private Mock<INotificationService> _mockNotificationService;
        private AppointmentService _appointmentService;

        [SetUp]
        public void SetUp()
        {
            _mockVetDbContext = new Mock<VetDbContext>();

            // Mock DbSet<Appointment>
            _mockAppointmentSet = new Mock<DbSet<Appointment>>();
            _mockUserSet = new Mock<DbSet<User>>();

            // Mock NotificationService
            _mockNotificationService = new Mock<INotificationService>();

            // Setup DbSet properties
            _mockVetDbContext.Setup(c => c.Appointments).Returns(_mockAppointmentSet.Object);
            _mockVetDbContext.Setup(c => c.Users).Returns(_mockUserSet.Object);

            // Initialize AppointmentService with mocks
            _appointmentService = new AppointmentService(_mockVetDbContext.Object, _mockNotificationService.Object);
        }

        [Test]
        public void BookAppointment_ShouldAddAppointmentToDatabase()
        {
            // Arrange
            var newAppointment = new AppointmentDTO
            {
                AppointmentDateTime = DateTime.Now,
                PetName = "Buddy",
                Reasons = "Regular Checkup"
            };
            var user = new User { Id = 1, Name = "John Doe" };

            _mockUserSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns(user);

            // Act
            var result = _appointmentService.BookAppointment(newAppointment, 1);

            // Assert
            _mockAppointmentSet.Verify(m => m.Add(It.IsAny<Appointment>()), Times.Once);
            _mockVetDbContext.Verify(m => m.SaveChanges(), Times.Once);
        }

        [Test]
        public void BookAppointment_ShouldThrowException_WhenUserNotFound()
        {
            // Arrange
            var newAppointment = new AppointmentDTO
            {
                AppointmentDateTime = DateTime.Now,
                PetName = "Buddy",
                Reasons = "Regular Checkup"
            };

            _mockUserSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns((User)null);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _appointmentService.BookAppointment(newAppointment, 1));
        }

        [Test]
        public void GetAppointmentById_ShouldReturnAppointment_WhenAppointmentExists()
        {
            // Arrange
            var appointment = new Appointment { AppointmentId = 1, ClientID = 1 };

            _mockAppointmentSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns(appointment);

            // Act
            var result = _appointmentService.GetAppointmentById(1);

            // Assert
            ClassicAssert.AreEqual(appointment, result);
        }

        [Test]
        public void RemoveAppointment_ShouldRemoveAppointmentFromDatabase()
        {
            // Arrange
            var appointment = new Appointment { AppointmentId = 1, ClientID = 1 };

            _mockAppointmentSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns(appointment);

            // Act
            _appointmentService.RemoveAppointment(1);

            // Assert
            _mockAppointmentSet.Verify(m => m.Remove(It.IsAny<Appointment>()), Times.Once);
            _mockVetDbContext.Verify(m => m.SaveChanges(), Times.Once);
            _mockNotificationService.Verify(m => m.Notification(It.IsAny<NotificationRequestDTO>()), Times.Once);
        }

        [Test]
        public void RemoveAppointment_ShouldThrowException_WhenAppointmentNotFound()
        {
            // Arrange
            _mockAppointmentSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns((Appointment)null);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _appointmentService.RemoveAppointment(1));
        }

        [Test]
        public void UpdateAppointment_ShouldUpdateAppointmentInDatabase()
        {
            // Arrange
            var appointment = new Appointment { AppointmentId = 1, ClientID = 1, AppointmentDateTime = DateTime.Now.AddDays(-1) };
            var updatedAppointment = new ManageAppointmentDTO { Id = 1, AppointmentDateTime = DateTime.Now };

            _mockAppointmentSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns(appointment);

            // Act
            _appointmentService.UpdateAppointment(updatedAppointment);

            // Assert
            _mockVetDbContext.Verify(m => m.SaveChanges(), Times.Once);
        }

        [Test]
        public void UpdateAppointment_ShouldThrowException_WhenAppointmentNotFound()
        {
            // Arrange
            var updatedAppointment = new ManageAppointmentDTO { Id = 1, AppointmentDateTime = DateTime.Now };

            _mockAppointmentSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns((Appointment)null);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _appointmentService.UpdateAppointment(updatedAppointment));
        }

        [Test]
        public void GetAppointmentsPerPage_ShouldReturnCorrectAppointments()
        {
            // Arrange
            var appointments = new List<Appointment>
            {
                new Appointment { AppointmentId = 1, ClientID = 1 },
                new Appointment { AppointmentId = 2, ClientID = 2 },
                new Appointment { AppointmentId = 3, ClientID = 3 }
            }.AsQueryable().BuildMockDbSet();

            _mockVetDbContext.Setup(c => c.Appointments).Returns(appointments.Object);

            // Act
            var result = _appointmentService.GetAppointmentsPerPage(1,2);

            // Assert
            ClassicAssert.AreEqual(2, result.Count);
        }

        [Test]
        public void GetUserAppointments_ShouldReturnCorrectAppointments()
        {
            // Arrange
            var appointments = new List<Appointment>
            {
                new Appointment { AppointmentId = 1, ClientID = 1 },
                new Appointment { AppointmentId = 2, ClientID = 1 },
                new Appointment { AppointmentId = 3, ClientID = 2 }
            }.AsQueryable().BuildMockDbSet();

            _mockVetDbContext.Setup(c => c.Appointments).Returns(appointments.Object);

            // Act
            var result = _appointmentService.GetUserAppointments(1, 2, 1);

            // Assert
            ClassicAssert.AreEqual(2, result.Count);
        }

        [Test]
        public void GetUserAppointmentsWOPagination_ShouldReturnAllAppointments()
        {
            // Arrange
            var appointments = new List<Appointment>
            {
                new Appointment { AppointmentId = 1, ClientID = 1 },
                new Appointment { AppointmentId = 2, ClientID = 1 },
                new Appointment { AppointmentId = 3, ClientID = 2 }
            }.AsQueryable().BuildMockDbSet();

            _mockVetDbContext.Setup(c => c.Appointments).Returns(appointments.Object);

            // Act
            var result = _appointmentService.GetUserAppointmentsWOPagination(1);

            // Assert
            ClassicAssert.AreEqual(2, result.Count);
        }
    }
}
