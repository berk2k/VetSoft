using Microsoft.AspNetCore.Mvc;
using TermProjectBackend.Models;
using TermProjectBackend.Models.Dto;
using TermProjectBackend.Source.Svc;

namespace TermProjectBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetPetController : ControllerBase
    {
        private readonly IPetService _petService;

        public GetPetController(IPetService petService)
        {
            _petService = petService;
        }

        [HttpGet("{ownerId}")]
        public IActionResult GetPetByOwnerId(int ownerId)
        {
            try
            {
                GetPetDTO petDTO = _petService.GetPetInformationById(ownerId);

                if (petDTO == null)
                {
                    return NotFound(new { Message = "Pet not found for the specified owner ID." });
                }

                return Ok(petDTO);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, new { Message = "An error occurred while retrieving pet information." });
            }
        }
    }
}
