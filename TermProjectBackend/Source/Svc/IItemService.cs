using TermProjectBackend.Models;
using TermProjectBackend.Models.Dto;

namespace TermProjectBackend.Source.Svc
{
    public interface IItemService
    {
        public Item AddItem (AddItemRequestDTO addItemRequestDTO);

        public void UpdateItem(UpdateItemRequestDTO updateItemRequestDTO);

        public List<Item> GetAllItems();
    }
}
