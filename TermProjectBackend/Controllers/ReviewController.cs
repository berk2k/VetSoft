using Microsoft.AspNetCore.Mvc;
using System.Net;
using TermProjectBackend.Models.Dto;
using TermProjectBackend.Models;
using TermProjectBackend.Source.Svc;

namespace TermProjectBackend.Controllers
{
    [Route("api/Review")]
    [ApiController]
    public class ReviewController : Controller
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;

        }
        [HttpPost("SendReview")]
        // [Authorize]
        public ActionResult SendReview([FromBody] ReviewRequestDTO reviewRequest)
        {
            try
            {
                var review = _reviewService.SendReview(reviewRequest);
                return Ok(new APIResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Status = "Success"
                });
            }
            catch (InvalidOperationException ex)
            {
                // Handle the case where the user is not found
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                return StatusCode(500, new { Message = "An error occurred while adding the record." });
            }
        }
    }
}
