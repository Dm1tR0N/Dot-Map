using DotMapApi.Models;

namespace DotMapApi.Interface;

public interface IReviewRepository
{
    IEnumerable<Review> GetReviewsForPlace(int placeId);
    IEnumerable<Review> GetReviewsByUserAndPlace(int userId, int placeId);
    Review GetReviewById(int reviewId);
    Review AddReview(Review review);
    void UpdateReview(Review review);
    void DeleteReview(int reviewId);
}