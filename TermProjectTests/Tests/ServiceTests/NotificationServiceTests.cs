using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using RabbitMQ.Client;
using System;
using System.Linq;
using TermProjectBackend.Context;
using TermProjectBackend.Models;
using TermProjectBackend.Models.Dto;
using TermProjectBackend.Source.Svc;

namespace TermProjectTests.Tests.ServiceTests
{
    [TestFixture]
    public class NotificationServiceTests
    {
        private Mock<VetDbContext> _mockVetDbContext;
        private Mock<DbSet<User>> _mockUserSet;
        private Mock<DbSet<Notification>> _mockNotificationSet;
        private Mock<DbSet<VeterinarianMessages>> _mockVeterinarianMessagesSet;
        private Mock<RabbitMqService> _mockRabbitMqService;
        private NotificationService _notificationService;

        [SetUp]
        public void SetUp()
        {
            // Mock VetDbContext
            _mockVetDbContext = new Mock<VetDbContext>();

            // Mock DbSet<User>
            _mockUserSet = new Mock<DbSet<User>>();

            // Mock DbSet<Notification>
            _mockNotificationSet = new Mock<DbSet<Notification>>();

            // Mock DbSet<VeterinarianMessages>
            _mockVeterinarianMessagesSet = new Mock<DbSet<VeterinarianMessages>>();

            // Mock RabbitMqService
            _mockRabbitMqService = new Mock<RabbitMqService>();

            // Setup DbContext mocks
            _mockVetDbContext.Setup(c => c.Users).Returns(_mockUserSet.Object);
            _mockVetDbContext.Setup(c => c.Notification).Returns(_mockNotificationSet.Object);
            _mockVetDbContext.Setup(c => c.VeterinarianMessages).Returns(_mockVeterinarianMessagesSet.Object);

            // Initialize NotificationService with mocks
            _notificationService = new NotificationService(_mockVetDbContext.Object);
        }


        [Test]
        public void GetName_ShouldReturnUserName_WhenUserExists()
        {
            // Arrange
            var userId = 1;
            var userName = "test_user";
            var user = new User { Id = userId, UserName = userName };

            _mockUserSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns(user);

            // Act
            var result = _notificationService.getName(userId);

            // Assert
            ClassicAssert.AreEqual(userName, result);
        }


        [Test]
        public void GetName_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = 1;

            _mockUserSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns((User)null);

            // Act
            var result = _notificationService.getName(userId);

            // Assert
            Assert.That(result, Is.Null);

        }

        [Test]
        public void Notification_ShouldAddNotificationToDatabase()
        {
            // Arrange
            var notificationRequest = new NotificationRequestDTO { userId = 1, message = "Test message" };
            var user = new User { Id = 1, UserName = "test_user" };
            _mockUserSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns(user);

            // Act
            _notificationService.Notification(notificationRequest);

            // Assert
            _mockNotificationSet.Verify(m => m.Add(It.IsAny<Notification>()), Times.Once);
            _mockVetDbContext.Verify(m => m.SaveChanges(), Times.Once);
        }



        [Test]
        public void Notification_ShouldThrowException_WhenUserNotFound()
        {
            // Arrange
            var notificationRequest = new NotificationRequestDTO { userId = 1, message = "Test message" };

            _mockUserSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns((User)null);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _notificationService.Notification(notificationRequest));
        }


        [Test]
        public void SendMessageToVet_ShouldAddMessageToDatabase()
        {
            // Arrange
            var vetMessageDTO = new VetMessageDTO { userId = 1, messageText = "Test message", messageTitle = "Test title" };
            var user = new User { Id = 1, UserName = "test_user" };

            _mockUserSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns(user);

            // Act
            _notificationService.SendMessageToVet(vetMessageDTO);

            // Assert
            _mockVeterinarianMessagesSet.Verify(m => m.Add(It.IsAny<VeterinarianMessages>()), Times.Once);
            _mockVetDbContext.Verify(m => m.SaveChanges(), Times.Once);
        }

        [Test]
        public void SendMessageToVet_ShouldThrowException_WhenUserNotFound()
        {
            // Arrange
            var vetMessageDTO = new VetMessageDTO { userId = 1, messageText = "Test message", messageTitle = "Test title" };

            _mockUserSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns((User)null);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _notificationService.SendMessageToVet(vetMessageDTO));
        }
    }
}
