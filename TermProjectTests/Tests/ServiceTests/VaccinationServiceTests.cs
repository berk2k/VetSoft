using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TermProjectBackend.Context;
using TermProjectBackend.Models;
using TermProjectBackend.Models.Dto;
using TermProjectBackend.Source.Svc;

namespace TermProjectTests.Tests.ServiceTests
{
    [TestFixture]
    public class VaccinationServiceTests
    {
        private Mock<VetDbContext> _mockVetDbContext;
        private Mock<DbSet<VaccinationRecord>> _mockVaccinationRecordSet;
        private Mock<DbSet<Pet>> _mockPetSet;
        private VaccinationService _vaccinationService;

        [SetUp]
        public void SetUp()
        {
            // Mock VetDbContext
            _mockVetDbContext = new Mock<VetDbContext>(new DbContextOptions<VetDbContext>());

            // Mock DbSet<VaccinationRecord>
            _mockVaccinationRecordSet = new Mock<DbSet<VaccinationRecord>>();
            _mockVetDbContext.Setup(c => c.VaccinationRecord).Returns(_mockVaccinationRecordSet.Object);

            // Mock DbSet<Pet>
            _mockPetSet = new Mock<DbSet<Pet>>();
            _mockVetDbContext.Setup(c => c.Pets).Returns(_mockPetSet.Object);

            // Initialize VaccinationService with mocks
            _vaccinationService = new VaccinationService(_mockVetDbContext.Object);
        }

        [Test]
        public void AddVaccinationRecord_ShouldAddRecord_WhenPetAndUserExist()
        {
            // Arrange
            var request = new AddVaccinationRecordRequestDTO
            {
                userId = 1,
                petId = 1,
                petName = "Pet1",
                vaccine_name = "Rabies",
                vaccine_date = DateTime.Now.ToString()
            };

            var pet1 = new Pet { Id = 1, OwnerID = 1, Name = "Pet1" };
 

            _mockPetSet.Setup(m => m.Find(It.Is<object[]>(ids => (int)ids[0] == 1))).Returns(pet1);


            // Act
            var result = _vaccinationService.AddVaccinationRecord(request);

            // Assert
            
            Assert.That(result.petName, Is.EqualTo("Pet1"));
            _mockVaccinationRecordSet.Verify(m => m.Add(It.IsAny<VaccinationRecord>()), Times.Once);
            _mockVetDbContext.Verify(m => m.SaveChanges(), Times.Once);
        }

        [Test]
        public void AddVaccinationRecord_ShouldThrowException_WhenPetOrUserNotFound()
        {
            // Arrange
            var request = new AddVaccinationRecordRequestDTO
            {
                userId = 1,
                petId = 1,
                petName = "Buddy",
                vaccine_name = "Rabies",
                vaccine_date = DateTime.Now.ToString()
            };

            var pets = new List<Pet>().AsQueryable(); // Empty list

            _mockPetSet.As<IQueryable<Pet>>().Setup(m => m.Provider).Returns(pets.Provider);
            _mockPetSet.As<IQueryable<Pet>>().Setup(m => m.Expression).Returns(pets.Expression);
            _mockPetSet.As<IQueryable<Pet>>().Setup(m => m.ElementType).Returns(pets.ElementType);
            _mockPetSet.As<IQueryable<Pet>>().Setup(m => m.GetEnumerator()).Returns(pets.GetEnumerator());

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _vaccinationService.AddVaccinationRecord(request));
        }

        

        

        [Test]
        public void RemoveVaccinationReport_ShouldRemoveRecord_WhenRecordExists()
        {
            // Arrange
            var recordId = 1;

            var record = new VaccinationRecord { vaccinationId = recordId, petName = "Buddy", vaccine_name = "Rabies", vaccine_date = DateTime.Now.ToString() };

            _mockVaccinationRecordSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns(record);

            // Act
            _vaccinationService.RemoveVaccinationReport(recordId);

            // Assert
            _mockVaccinationRecordSet.Verify(m => m.Remove(It.IsAny<VaccinationRecord>()), Times.Once);
            _mockVetDbContext.Verify(m => m.SaveChanges(), Times.Once);
        }

        [Test]
        public void RemoveVaccinationReport_ShouldThrowException_WhenRecordNotFound()
        {
            // Arrange
            var recordId = 1;

            _mockVaccinationRecordSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns((VaccinationRecord)null);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _vaccinationService.RemoveVaccinationReport(recordId));
        }
    }
}
