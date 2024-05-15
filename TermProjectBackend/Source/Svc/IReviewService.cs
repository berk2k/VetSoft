using TermProjectBackend.Models;
using TermProjectBackend.Models.Dto;

namespace TermProjectBackend.Source.Svc
{
    public interface IReviewService
    {
        public Review SendReview(ReviewRequestDTO requestDTO);

        public string GetPetNameById(int id);

        public string GetUserNameById(int id);

        public List<Review> GetAllReviews(int page, int pageSize);

        public List<Review> GetUserReviews(int page, int pageSize, int userId);
    }
}
