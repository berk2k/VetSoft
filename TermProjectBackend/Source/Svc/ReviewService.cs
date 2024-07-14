using Azure.Core;
using TermProjectBackend.Context;
using TermProjectBackend.Models;
using TermProjectBackend.Models.Dto;

namespace TermProjectBackend.Source.Svc
{
    public class ReviewService : IReviewService
    {
        private readonly VetDbContext _vetDb;
        public ReviewService(VetDbContext vetDb)
        {

            _vetDb = vetDb;

        }

        public List<Review> GetAllReviews(int page, int pageSize)
        {
            return _vetDb.Reviews
                .Skip((page - 1) * pageSize)
                .AsQueryable()
                .Take(pageSize)
                .ToList();
        }

        public string GetPetNameById(int id)
        {
            var pet = _vetDb.Pets.Find(id);

            if (pet != null)
            {
                return pet.Name;
            }
            else
            {
                throw new ArgumentException("No pet found with the provided ID.");
            }
        }


        public string GetUserNameById(int id)
        {
            var user = _vetDb.Users.Find(id);

            // Check if a user with the given ID exists
            if (user != null)
            {
                // Return the user name
                return user.Name;
            }
            else
            {
                // If no user found with the given ID, throw an exception or return null, depending on your requirements
                throw new ArgumentException("No user found with the provided ID.");
                // Alternatively, you can return null
                // return null;
            }
        }

        public List<Review> GetUserReviews(int page, int pageSize, int userId)
        {
            return _vetDb.Reviews
                .Where(review => review.userId == userId)
                .AsQueryable()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public List<Review> GetUserReviewsWOPagination(int userId)
        {
            return _vetDb.Reviews
                .Where(review => review.userId == userId)
                .AsQueryable()
                .ToList();
        }

        public Review SendReview(ReviewRequestDTO requestDTO)
        {
            DateTime utcNow = DateTime.UtcNow;
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");
            DateTime trTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, tzi);

            // Check if the pet exists and belongs to the specified user
            var pet = _vetDb.Pets.Find(requestDTO.petId);
            if (pet == null || pet.OwnerID != requestDTO.userId)
            {
                throw new InvalidOperationException("pet or user not found.");
            }

            Review newReview = new Review
            {
                message = requestDTO.message,
                userId = requestDTO.userId,
                petId = requestDTO.petId,
                userName = GetUserNameById(requestDTO.userId),
                petName = pet.Name,
                SentAt = trTime
            };

            _vetDb.Reviews.Add(newReview);
            _vetDb.SaveChanges();

            return newReview;
        }

    }
}
