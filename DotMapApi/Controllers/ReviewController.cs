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

    // [HttpGet]
    // public IActionResult GetAllPlaces()
    // {
    //     var places = _reviewRepository.GetAllPlaces();
    //     return Ok(places);
    // }
    //
    // [HttpGet("{id}")]
    // public IActionResult GetPlaceById(int id)
    // {
    //     var place = _reviewRepository.GetPlaceById(id);
    //     if (place == null)
    //     {
    //         return NotFound();
    //     }
    //     return Ok(place);
    // }
    //
    // [HttpPost]
    // public IActionResult AddPlace(Place place)
    // {
    //     var addedPlace = _reviewRepository.AddPlace(place);
    //     return CreatedAtAction(nameof(GetPlaceById), new { id = addedPlace.Id }, addedPlace);
    // }
    //
    // [HttpPut("{id}")]
    // public IActionResult UpdatePlace(int id, Place place)
    // {
    //     if (id != place.Id)
    //     {
    //         return BadRequest();
    //     }
    //
    //     _reviewRepository.UpdatePlace(place);
    //     return NoContent();
    // }
    //
    // [HttpDelete("{id}")]
    // public IActionResult DeletePlace(int id)
    // {
    //     var place = _reviewRepository.GetPlaceById(id);
    //     if (place == null)
    //     {
    //         return NotFound();
    //     }
    //
    //     _reviewRepository.DeletePlace(id);
    //     return NoContent();
    // }

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