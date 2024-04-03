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
        public string GetPetNameById(int id)
        {
            // Query the database to retrieve the pet name by ID
            var pet = _vetDb.Pets.FirstOrDefault(p => p.Id == id);

            // Check if a pet with the given ID exists
            if (pet != null)
            {
                // Return the pet name
                return pet.Name;
            }
            else
            {
                // If no pet found with the given ID, throw an exception or return null, depending on your requirements
                throw new ArgumentException("No pet found with the provided ID.");
                // Alternatively, you can return null
                // return null;
            }
        }

        public string GetUserNameById(int id)
        {
            var user = _vetDb.Users.FirstOrDefault(u => u.Id == id);

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

        public Review SendReview(ReviewRequestDTO requestDTO)
        {
            if (!_vetDb.Pets.Any(p => p.Id == requestDTO.petId && p.OwnerID == requestDTO.userId))
            {
                throw new InvalidOperationException("pet or user not found.");
            }
            // Create a new Notification instance
            Review newReview = new Review
            {
                message = requestDTO.message,
                userId = requestDTO.userId,
                petId = requestDTO.petId,
                userName = GetUserNameById(requestDTO.userId),
                petName = GetPetNameById(requestDTO.petId)
            };

            // Add the new notification to the Notifications DbSet
            _vetDb.Reviews.Add(newReview);

            // Save changes to the database
            _vetDb.SaveChanges();

            return newReview;
        }
    }
}
