using TermProjectBackend.Models;
using TermProjectBackend.Models.Dto;

namespace TermProjectBackend.Source.Svc
{
    public interface IItemService
    {
        public Item AddItem (AddItemRequestDTO addItemRequestDTO);

        public void UpdateItem(UpdateItemRequestDTO updateItemRequestDTO);

        public List<Item> GetAllItems();

        List<Item> GetItemsPerPage(int page, int pageSize);

        public List<Item> GetItemByName (string medicineName);

        public List<Item> GetOutOfStockItems();
    }
}
