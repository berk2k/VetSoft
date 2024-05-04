using Microsoft.AspNetCore.Mvc;
using System.Net;
using TermProjectBackend.Models;
using TermProjectBackend.Models.Dto;
using TermProjectBackend.Source.Svc;

namespace TermProjectBackend.Controllers
{
    [Route("api/Stock")]
    [ApiController]
    public class StockController : Controller
    {
        private readonly IItemService _itemService;
        public StockController(IItemService itemService) {

            _itemService = itemService;
        }

        [HttpPost("Add Item")]
        public ActionResult AddItem([FromBody] AddItemRequestDTO requestDTO)
        {
            

            if (requestDTO.Count < 0)
            {
                return BadRequest(new APIResponse
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    IsSuccess = false,
                    Status = "Fail",
                    ErrorMessage = "Count can not be lower than 0"
                });
            }

            var item = _itemService.AddItem(requestDTO);

            if (item == null)
            {
                return BadRequest(new APIResponse
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    IsSuccess = false,
                    Status = "Fail",
                    ErrorMessage = "Error adding item"
                });
            }

            return Ok(new APIResponse
            {
                StatusCode = HttpStatusCode.OK,
                IsSuccess = true,
                Status = "Success"
            });
        }

        [HttpPost("update")]
        public ActionResult UpdateItem([FromBody] UpdateItemRequestDTO requestDTO)
        {

            

            if (requestDTO.id == 0)
            {
                return BadRequest(new APIResponse
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    IsSuccess = false,
                    Status = "Fail",
                    ErrorMessage = "Error update item. check id"
                });
            }

            if (requestDTO.Count < 0)
            {
                return BadRequest(new APIResponse
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    IsSuccess = false,
                    Status = "Fail",
                    ErrorMessage = "Count can not be lower than 0"
                });
            }

            try
            {
                _itemService.UpdateItem(requestDTO);
                return Ok(new APIResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Status = "Success"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the item: {ex.Message}");
            }
        }

        [HttpGet("GetAllItems")]
        public ActionResult<List<Item>> GetAllItems(int page = 1)
        {
            try
            {
                var items = _itemService.GetItemsPerPage(page, 10);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while fetching items: {ex.Message}");
            }
        }

        [HttpGet("GetItemByName")]
        public ActionResult<Item> GetItemByName(string medicineName)
        {
            try
            {
                var item = _itemService.GetItemByName(medicineName);

                if (item != null)
                {
                    return Ok(item); 
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while getting item by name: {ex.Message}");
            }
        }

        [HttpGet("GetItemsWithZeroCount")]
        public ActionResult<List<Item>> GetItemsWithZeroCount()
        {
            try
            {
                var items = _itemService.GetOutOfStockItems();

                if (items.Any())
                {
                    return Ok(items); 
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while fetching items with zero count: {ex.Message}"); 
            }
        }







    }
}
