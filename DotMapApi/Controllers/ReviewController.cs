using Microsoft.AspNetCore.Mvc;
using DotMapApi.Models;
using DotMapApi.Interface;
using DotMapApi.Interface.Classes;

namespace DotMapApi.Controllers;

[ApiController]
[Route("api/places")]
public class ReviewController : ControllerBase
{
    private readonly IReviewRepository _reviewRepository;

    public ReviewController(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    [HttpGet("{placeId}/reviews")]
    public IActionResult GetReviewsForPlace(int placeId)
    {
        var reviews = _reviewRepository.GetReviewsForPlace(placeId);
        return Ok(reviews);
    }

    [HttpGet("{placeId}/users/{userId}/reviews")]
    public IActionResult GetReviewsByUserAndPlace(int placeId, int userId)
    {
        var reviews = _reviewRepository.GetReviewsByUserAndPlace(userId, placeId);
        return Ok(reviews);
    }

    [HttpGet("reviews/{reviewId}")]
    public IActionResult GetReviewById(int reviewId)
    {
        var review = _reviewRepository.GetReviewById(reviewId);
        if (review == null)
        {
            return NotFound();
        }
        return Ok(review);
    }

    [HttpPost("reviews")]
    public IActionResult AddReview(Review review)
    {
        var addedReview = _reviewRepository.AddReview(review);
        return CreatedAtAction(nameof(GetReviewById), new { reviewId = addedReview.Id }, addedReview);
    }

    [HttpPut("reviews/{reviewId}")]
    public IActionResult UpdateReview(int reviewId, Review review)
    {
        if (reviewId != review.Id)
        {
            return BadRequest();
        }

        _reviewRepository.UpdateReview(review);
        return NoContent();
    }

    [HttpDelete("reviews/{reviewId}")]
    public IActionResult DeleteReview(int reviewId)
    {
        var review = _reviewRepository.GetReviewById(reviewId);
        if (review == null)
        {
            return NotFound();
        }

        _reviewRepository.DeleteReview(reviewId);
        return NoContent();
    }
}
