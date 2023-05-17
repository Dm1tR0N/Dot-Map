using System.Collections.Generic;
using System.Linq;
using DotMapApi.Interface;
using DotMapApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DotMapApi.Interface.Classes;

public class ReviewRepository : IReviewRepository
{
    private readonly ApplicationDbContext _context;
    
    public ReviewRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    public IEnumerable<Review> GetReviewsForPlace(int placeId)
    {
        var reviews = _context.Reviews.Where(r => r.PlaceId == placeId).ToList();
        return reviews;
    }

    public IEnumerable<Review> GetReviewsByUserAndPlace(int userId, int placeId)
    {
        var reviews = _context.Reviews.Where(r => r.User == userId && r.PlaceId == placeId).ToList();
        return reviews;
    }

    public Review GetReviewById(int reviewId)
    {
        return _context.Reviews.SingleOrDefault(r => r.Id == reviewId);
    }

    public Review AddReview(Review review)
    {
        _context.Reviews.Add(review);
        _context.SaveChanges();
        return review;
    }

    public void UpdateReview(Review review)
    {
        _context.Entry(review).State = EntityState.Modified;
        _context.SaveChanges();
    }

    public void DeleteReview(int reviewId)
    {
        var review = _context.Reviews.FirstOrDefault(r => r.Id == reviewId);
        if (review != null)
        {
            _context.Reviews.Remove(review);
            _context.SaveChanges();
        }
    }
}