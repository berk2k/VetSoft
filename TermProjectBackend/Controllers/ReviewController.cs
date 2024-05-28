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

        [HttpGet("GetAllReviews")]
        public ActionResult<List<Review>> GetAllReviews(int page = 1)
        {
            try
            {
                var items = _reviewService.GetAllReviews(page, 10);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while fetching items: {ex.Message}");
            }
        }

        [HttpGet("GetReviewsHistoryForUser")]
        public ActionResult<List<Review>> GetReviewsForUser(int page = 1, int userId = 0)
        {
            try
            {
                var reviews = _reviewService.GetUserReviews(page, 10, userId);
                var userReviews = reviews.Select(a => new ReviewRequestDTO
                {
                    userId = a.userId,
                    petId = a.petId,
                    message = a.message,
                    SentAt = a.SentAt
                }).ToList();
                return Ok(userReviews);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while fetching items: {ex.Message}");
            }
        }

        [HttpGet("GetReviewsHistoryForUserWOPagination")]
        public ActionResult<List<Review>> GetReviewsForUserWOPagination(int userId = 0)
        {
            try
            {
                var reviews = _reviewService.GetUserReviewsWOPagination(userId);
                var userReviews = reviews.Select(a => new ReviewRequestDTO
                {
                    userId = a.userId,
                    petId = a.petId,
                    message = a.message,
                    SentAt = a.SentAt
                }).ToList();
                return Ok(userReviews);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while fetching items: {ex.Message}");
            }
        }
    }
}
