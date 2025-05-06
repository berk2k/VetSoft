using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using TermProjectBackend.Context;
using TermProjectBackend.Models;
using TermProjectBackend.Models.Dto;

namespace TermProjectBackend.Source.Svc
{
    public class ItemService : IItemService
    {
        private readonly ILogger<ItemService> _logger;
        private readonly VetDbContext _vetDb;

        public ItemService(VetDbContext vetDb, ILogger<ItemService> logger)
        {
            _vetDb = vetDb;
            _logger = logger;   

        }

        public Item AddItem(AddItemRequestDTO addItemRequestDTO)
        {
            try
            {
                _logger.LogInformation("AddItem method started. ItemName: {ItemName}, Count: {Count}", addItemRequestDTO.ItemName, addItemRequestDTO.Count);

                var newItem = new Item
                {
                    medicine_name = addItemRequestDTO.ItemName,
                    count = addItemRequestDTO.Count
                };

                _vetDb.Items.Add(newItem);
                _vetDb.SaveChanges();

                _logger.LogInformation("Item successfully added. ItemId: {ItemId}", newItem.id);

                return newItem;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding item. ItemName: {ItemName}, Count: {Count}", addItemRequestDTO.ItemName, addItemRequestDTO.Count);
                throw new Exception($"Error occurred while adding item: {ex.Message}");
            }
        }





        public void UpdateItem(UpdateItemRequestDTO updateItemRequestDTO)
        {
            try
            {
                _logger.LogInformation("UpdateItem method started. ItemId: {ItemId}, NewItemName: {ItemName}, NewCount: {Count}",
                    updateItemRequestDTO.id, updateItemRequestDTO.ItemName, updateItemRequestDTO.Count);

                var itemToUpdate = _vetDb.Items.Find(updateItemRequestDTO.id);

                if (itemToUpdate != null)
                {
                    itemToUpdate.medicine_name = updateItemRequestDTO.ItemName;
                    itemToUpdate.count = updateItemRequestDTO.Count;

                    _vetDb.SaveChanges();

                    _logger.LogInformation("Item successfully updated. ItemId: {ItemId}", updateItemRequestDTO.id);
                }
                else
                {
                    _logger.LogWarning("Item not found for update. ItemId: {ItemId}", updateItemRequestDTO.id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating item. ItemId: {ItemId}", updateItemRequestDTO.id);
                throw new Exception($"Error occurred while updating item: {ex.Message}");
            }
        }



        public List<Item> GetAllItems()
        {
            return _vetDb.Items.ToList();
        }

        public List<Item> GetItemsPerPage(int page, int pageSize)
        {
            return _vetDb.Items
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public List<Item> GetItemByName(string medicineName)
        {
            try
            {
                _logger.LogInformation("GetItemByName method started. MedicineName: {MedicineName}", medicineName);

                medicineName = medicineName.ToLower();
                var items = _vetDb.Items
                    .Where(i => i.medicine_name.ToLower().Contains(medicineName))
                    .AsQueryable()
                    .ToList();

                if (items != null && items.Any())
                {
                    _logger.LogInformation("Item search completed. Found: {ItemCount} item(s).", items.Count);
                    return items;
                }
                else
                {
                    _logger.LogWarning("No items found. MedicineName: {MedicineName}", medicineName);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching for item. MedicineName: {MedicineName}", medicineName);
                throw new Exception($"Error occurred while getting item by name: {ex.Message}");
            }
        }


        public List<Item> GetOutOfStockItems()
        {
            try
            {
                _logger.LogInformation("GetOutOfStockItems method started.");

                var itemsWithZeroCount = _vetDb.Items
                    .Where(i => i.count == 0)
                    .AsQueryable()
                    .ToList();

                _logger.LogInformation("Out of stock items search completed. Found: {ItemCount} item(s).", itemsWithZeroCount.Count);

                return itemsWithZeroCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching for out of stock items.");
                throw new Exception($"Error occurred while getting out of stock items: {ex.Message}");
            }
        }

    }
}
