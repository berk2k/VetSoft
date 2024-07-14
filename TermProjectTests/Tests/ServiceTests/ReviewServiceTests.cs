using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using TermProjectBackend.Context;
using TermProjectBackend.Models;
using TermProjectBackend.Models.Dto;
using TermProjectBackend.Source.Svc;

namespace TermProjectBackend.Tests
{
    [TestFixture]
    public class ReviewServiceTests
    {
        private Mock<VetDbContext> _mockVetDbContext;
        private Mock<DbSet<Review>> _mockReviewSet;
        private Mock<DbSet<Pet>> _mockPetSet;
        private Mock<DbSet<User>> _mockUserSet;
        private ReviewService _reviewService;

        [SetUp]
        public void SetUp()
        {
            _mockVetDbContext = new Mock<VetDbContext>();
            _mockReviewSet = new Mock<DbSet<Review>>();
            _mockPetSet = new Mock<DbSet<Pet>>();
            _mockUserSet = new Mock<DbSet<User>>();

            _mockVetDbContext.Setup(db => db.Reviews).Returns(_mockReviewSet.Object);
            _mockVetDbContext.Setup(db => db.Pets).Returns(_mockPetSet.Object);
            _mockVetDbContext.Setup(db => db.Users).Returns(_mockUserSet.Object);

            _reviewService = new ReviewService(_mockVetDbContext.Object);
        }

        [Test]
        public void GetAllReviews_ShouldReturnCorrectReviews()
        {
            // Arrange
            var reviews = new List<Review>
            {
                new Review { reviewId = 1, userId = 1, petId = 1 },
                new Review { reviewId = 2, userId = 2, petId = 2 }
            }.AsQueryable().BuildMockDbSet();

            _mockVetDbContext.Setup(c => c.Reviews).Returns(reviews.Object);

            // Act
            var result = _reviewService.GetAllReviews(1, 2);

            // Assert
            ClassicAssert.AreEqual(2, result.Count);
        }

        [Test]
        public void GetUserNameById_ShouldReturnCorrectUserName()
        {
            // Arrange
            var user = new User { Id = 1, Name = "User1" };
            var user2 = new User { Id = 2, Name = "User2" };



            _mockUserSet.Setup(m => m.Find(It.Is<object[]>(ids => (int)ids[0] == 1))).Returns(user);
            _mockUserSet.Setup(m => m.Find(It.Is<object[]>(ids => (int)ids[0] == 2))).Returns(user2);

            // Act
            var result = _reviewService.GetUserNameById(1);

            // Assert
            ClassicAssert.AreEqual("User1", result);

            // Act
            var result2 = _reviewService.GetUserNameById(2);

            // Assert
            ClassicAssert.AreEqual("User2", result2);
        }

        [Test]
        public void GetPetNameById_ShouldReturnCorrectPetName()
        {
            // Arrange
            var pet1 = new Pet { Id = 1, Name = "Pet1" };
            var pet2 = new Pet { Id = 2, Name = "Pet2" };

            _mockPetSet.Setup(m => m.Find(It.Is<object[]>(ids => (int)ids[0] == 1))).Returns(pet1);
            _mockPetSet.Setup(m => m.Find(It.Is<object[]>(ids => (int)ids[0] == 2))).Returns(pet2);

            // Act & Assert
            var result1 = _reviewService.GetPetNameById(1);
            ClassicAssert.AreEqual("Pet1", result1);

            var result2 = _reviewService.GetPetNameById(2);
            ClassicAssert.AreEqual("Pet2", result2);

           
        }


        [Test]
        public void SendReview_ShouldCreateAndReturnReview()
        {
            var pet1 = new Pet { Id = 1, OwnerID = 1, Name = "Pet1" };
            var pet2 = new Pet { Id = 2, OwnerID = 1, Name = "Pet2" };

            _mockPetSet.Setup(m => m.Find(It.Is<object[]>(ids => (int)ids[0] == 1))).Returns(pet1);
            _mockPetSet.Setup(m => m.Find(It.Is<object[]>(ids => (int)ids[0] == 2))).Returns(pet2);

            var user = new User { Id = 1, Name = "User1" };
            var user2 = new User { Id = 2, Name = "User2" };



            _mockUserSet.Setup(m => m.Find(It.Is<object[]>(ids => (int)ids[0] == 1))).Returns(user);
            _mockUserSet.Setup(m => m.Find(It.Is<object[]>(ids => (int)ids[0] == 2))).Returns(user2);

     

            var reviewRequest = new ReviewRequestDTO
            {
                message = "Great service!",
                userId = 1,
                petId = 2
            };

            // Act
            var result = _reviewService.SendReview(reviewRequest);

            // Assert
            ClassicAssert.AreEqual(reviewRequest.message, result.message);
            ClassicAssert.AreEqual(reviewRequest.userId, result.userId);
            ClassicAssert.AreEqual(reviewRequest.petId, result.petId);
            ClassicAssert.AreEqual("User1", result.userName);
            ClassicAssert.AreEqual("Pet2", result.petName);

            
        }
    }

    // Helper extension to mock DbSet
    public static class MockDbSetExtensions
    {
        public static Mock<DbSet<T>> BuildMockDbSet<T>(this IQueryable<T> source) where T : class
        {
            var mock = new Mock<DbSet<T>>();
            mock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(source.Provider);
            mock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(source.Expression);
            mock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(source.ElementType);
            mock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(source.GetEnumerator());
            return mock;
        }
    }
}