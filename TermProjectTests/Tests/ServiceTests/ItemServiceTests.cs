using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TermProjectBackend.Context;
using TermProjectBackend.Models;
using TermProjectBackend.Models.Dto;
using TermProjectBackend.Source.Svc;
using NUnit.Framework.Legacy;
using MockQueryable.Moq;

namespace TermProjectBackend.Tests
{
    [TestFixture]
    public class ItemServiceTests
    {
        private Mock<VetDbContext> _mockVetDbContext;
        private Mock<DbSet<Item>> _mockItemSet;
        private ItemService _itemService;

        [SetUp]
        public void SetUp()
        {
            // Mock DbSet<Item>
            _mockItemSet = new Mock<DbSet<Item>>();

            // Mock VetDbContext
            _mockVetDbContext = new Mock<VetDbContext>();

            _mockVetDbContext.Setup(c => c.Items).Returns(_mockItemSet.Object);

            // Initialize ItemService with mocked context
            _itemService = new ItemService(_mockVetDbContext.Object);
        }

        [Test]
        public void AddItem_ShouldAddNewItem()
        {
            // Arrange
            var addItemRequestDTO = new AddItemRequestDTO
            {
                ItemName = "Medicine A",
                Count = 10
            };

            var items = new List<Item>().AsQueryable();

            _mockItemSet.As<IQueryable<Item>>().Setup(m => m.Provider).Returns(items.Provider);
            _mockItemSet.As<IQueryable<Item>>().Setup(m => m.Expression).Returns(items.Expression);
            _mockItemSet.As<IQueryable<Item>>().Setup(m => m.ElementType).Returns(items.ElementType);
            _mockItemSet.As<IQueryable<Item>>().Setup(m => m.GetEnumerator()).Returns(items.GetEnumerator());

            _mockItemSet.Setup(m => m.Add(It.IsAny<Item>())).Callback<Item>((item) => items.ToList().Add(item));

            // Act
            var result = _itemService.AddItem(addItemRequestDTO);

            // Assert
            _mockItemSet.Verify(m => m.Add(It.IsAny<Item>()), Times.Once);
            _mockVetDbContext.Verify(m => m.SaveChanges(), Times.Once);
            ClassicAssert.AreEqual(addItemRequestDTO.ItemName, result.medicine_name);
            ClassicAssert.AreEqual(addItemRequestDTO.Count, result.count);
        }

        [Test]
        public void UpdateItem_ShouldUpdateExistingItem()
        {
            // Arrange
            var updateItemRequestDTO = new UpdateItemRequestDTO
            {
                id = 1,
                ItemName = "Updated Medicine",
                Count = 20
            };

            var existingItem = new Item { id = 1, medicine_name = "Old Medicine", count = 10 };
            var items = new List<Item> { existingItem }.AsQueryable();

            _mockItemSet.As<IQueryable<Item>>().Setup(m => m.Provider).Returns(items.Provider);
            _mockItemSet.As<IQueryable<Item>>().Setup(m => m.Expression).Returns(items.Expression);
            _mockItemSet.As<IQueryable<Item>>().Setup(m => m.ElementType).Returns(items.ElementType);
            _mockItemSet.As<IQueryable<Item>>().Setup(m => m.GetEnumerator()).Returns(items.GetEnumerator());

            _mockItemSet.Setup(m => m.Find(It.IsAny<int>())).Returns(existingItem);

            // Act
            _itemService.UpdateItem(updateItemRequestDTO);

            // Assert
            ClassicAssert.AreEqual(updateItemRequestDTO.ItemName, existingItem.medicine_name);
            ClassicAssert.AreEqual(updateItemRequestDTO.Count, existingItem.count);
            _mockVetDbContext.Verify(m => m.SaveChanges(), Times.Once);
        }

        [Test]
        public void GetAllItems_ShouldReturnAllItems()
        {
            // Arrange
            var items = new List<Item>
            {
                new Item { id = 1, medicine_name = "Medicine A", count = 10 },
                new Item { id = 2, medicine_name = "Medicine B", count = 5 }
            }.AsQueryable().BuildMockDbSet();

            _mockVetDbContext.Setup(c => c.Items).Returns(items.Object);

            // Act
            var result = _itemService.GetAllItems();

            // Assert
            ClassicAssert.AreEqual(2, result.Count);
        }

        [Test]
        public void GetItemsPerPage_ShouldReturnCorrectItems()
        {
            // Arrange
            var items = new List<Item>
            {
                new Item { id = 1, medicine_name = "Medicine A", count = 10 },
                new Item { id = 2, medicine_name = "Medicine B", count = 5 },
                new Item { id = 3, medicine_name = "Medicine C", count = 3 }
            }.AsQueryable().BuildMockDbSet();

            _mockVetDbContext.Setup(c => c.Items).Returns(items.Object);

            // Act
            var result = _itemService.GetItemsPerPage(1, 2);

            // Assert
            ClassicAssert.AreEqual(2, result.Count);
        }

        [Test]
        public void GetItemByName_ShouldReturnCorrectItems()
        {
            // Arrange
            var items = new List<Item>
            {
                new Item { id = 1, medicine_name = "Medicine A", count = 10 },
                new Item { id = 2, medicine_name = "Medicine B", count = 5 },
                new Item { id = 3, medicine_name = "Another Medicine", count = 3 }
            }.AsQueryable().BuildMockDbSet();

            _mockVetDbContext.Setup(c => c.Items).Returns(items.Object);

            // Act
            var result = _itemService.GetItemByName("Medicine");

            // Assert
            ClassicAssert.AreEqual(3, result.Count);
        }

        [Test]
        public void GetOutOfStockItems_ShouldReturnItemsWithZeroCount()
        {
            // Arrange
            var items = new List<Item>
            {
                new Item { id = 1, medicine_name = "Medicine A", count = 0 },
                new Item { id = 2, medicine_name = "Medicine B", count = 5 },
                new Item { id = 3, medicine_name = "Medicine C", count = 0 }
            }.AsQueryable().BuildMockDbSet();

            _mockVetDbContext.Setup(c => c.Items).Returns(items.Object);

            // Act
            var result = _itemService.GetOutOfStockItems();

            // Assert
            ClassicAssert.AreEqual(2, result.Count);
        }
    }
}
