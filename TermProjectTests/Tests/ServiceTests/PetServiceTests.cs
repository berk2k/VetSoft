using Moq;
using NUnit.Framework;
using MockQueryable.Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TermProjectBackend.Context;
using TermProjectBackend.Models;
using TermProjectBackend.Models.Dto;
using TermProjectBackend.Source.Svc;
using NUnit.Framework.Legacy;

namespace TermProjectBackend.Tests
{
    [TestFixture]
    public class PetServiceTests
    {
        private Mock<VetDbContext> _mockVetDbContext;
        private Mock<DbSet<Pet>> _mockPetSet;
        private PetService _petService;

        [SetUp]
        public void SetUp()
        {
            _mockVetDbContext = new Mock<VetDbContext>();
            _mockPetSet = new Mock<DbSet<Pet>>();

            _mockVetDbContext.Setup(c => c.Pets).Returns(_mockPetSet.Object);

            _petService = new PetService(_mockVetDbContext.Object);
        }

        [Test]
        public void GetPetInformationById_ShouldReturnCorrectPetDTOs()
        {
            // Arrange
            var pets = new List<Pet>
            {
                new Pet { Id = 1, OwnerID = 1, Name = "Max", Breed = "Bulldog", Gender = "Male", Weight = 20, Species = "Dog", Age = 3, Allergies = "None" },
                new Pet { Id = 2, OwnerID = 1, Name = "Bella", Breed = "Beagle", Gender = "Female", Weight = 15, Species = "Dog", Age = 2, Allergies = "Pollen" }
            }.AsQueryable().BuildMockDbSet();

            _mockVetDbContext.Setup(c => c.Pets).Returns(pets.Object);

            // Act
            var result = _petService.GetPetInformationById(1);

            // Assert
            ClassicAssert.AreEqual(2, result.Count);
            ClassicAssert.AreEqual("Max", result[0].Name);
            ClassicAssert.AreEqual("Bella", result[1].Name);
        }

        [Test]
        public void AddPet_ShouldAddPetAndReturnPet()
        {
            // Arrange
            var addPetRequest = new AddPetRequestDTO
            {
                id = 1,
                Name = "Max",
                Species = "Dog",
                Breed = "Bulldog",
                Color = "Brown",
                Age = 3,
                Gender = "Male",
                Weight = 20,
                Allergies = "None"
            };

            // Act
            var result = _petService.AddPet(addPetRequest);

            // Assert
            _mockPetSet.Verify(m => m.Add(It.IsAny<Pet>()), Times.Once);
            _mockVetDbContext.Verify(m => m.SaveChanges(), Times.Once);

            ClassicAssert.AreEqual("Max", result.Name);
        }

        [Test]
        public void IsPetUnique_ShouldReturnTrueIfPetIsUnique()
        {
            // Arrange
            _mockPetSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns((Pet)null);

            // Act
            var result = _petService.IsPetUnique("Max");

            // Assert
            ClassicAssert.IsTrue(result);
        }

        [Test]
        public void IsPetUnique_ShouldReturnFalseIfPetIsNotUnique()
        {
            // Arrange
            var pet = new Pet { Name = "Max" };
            _mockPetSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns(pet);

            // Act
            var result = _petService.IsPetUnique("Max");

            // Assert
            ClassicAssert.IsFalse(result);
        }

        [Test]
        public void GetPetNameById_ShouldReturnCorrectPetName()
        {
            // Arrange
            var pet = new Pet { Id = 1, Name = "Max" };
            _mockPetSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns(pet);

            // Act
            var result = _petService.GetPetNameById(1);

            // Assert
            ClassicAssert.AreEqual("Max", result);
        }

        [Test]
        public void GetPetNameById_ShouldThrowExceptionIfPetNotFound()
        {
            // Arrange
            _mockPetSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns((Pet)null);

            // Act & Assert
            ClassicAssert.Throws<ArgumentException>(() => _petService.GetPetNameById(1));
        }
    }
}
