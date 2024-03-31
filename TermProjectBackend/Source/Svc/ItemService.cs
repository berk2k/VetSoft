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
        private readonly VetDbContext _vetDb;

        public ItemService(VetDbContext vetDb)
        {
            _vetDb = vetDb;
        }

        public Item AddItem(AddItemRequestDTO addItemRequestDTO)
        {
           

            // Create a new item
            var newItem = new Item
            {
                medicine_name = addItemRequestDTO.ItemName,
                count = addItemRequestDTO.Count
            };

            // Add the new item to the database context and save changes
            _vetDb.Items.Add(newItem);
            _vetDb.SaveChanges();

            // Return the newly added item
            return newItem;
        }



        public void UpdateItem(UpdateItemRequestDTO updateItemRequestDTO)
        {
            // Check if an item with the same name already exists
            //var existingItem = _vetDb.Items.FirstOrDefault(i => i.medicine_name == updateItemRequestDTO.ItemName && i.id != updateItemRequestDTO.id);

            //if (existingItem != null)
            //{
            //    throw new Exception("An item with the same name already exists.");
            //}

            // Check if the count is lower than 0
            //if (updateItemRequestDTO.Count < 0)
            //{
            //    throw new ArgumentException("Count cannot be lower than 0.");
            //}

            var itemToUpdate = _vetDb.Items.FirstOrDefault(i => i.id == updateItemRequestDTO.id);

            if (itemToUpdate != null)
            {
                itemToUpdate.medicine_name = updateItemRequestDTO.ItemName;
                itemToUpdate.count = updateItemRequestDTO.Count;

                _vetDb.SaveChanges();
            }
            
        }


        public List<Item> GetAllItems()
        {
            return _vetDb.Items.ToList();
        }
    }
}
