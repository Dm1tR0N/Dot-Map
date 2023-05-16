using Microsoft.AspNetCore.Mvc;
using DotMapApi.Models;
using DotMapApi.Interface;

namespace DotMapApi.Controllers;

[Route("api/places")]
public class PlaceController : ControllerBase
{
    private readonly IPlaceRepository _placeRepository;

    public PlaceController(IPlaceRepository placeRepository)
    {
        _placeRepository = placeRepository;
    }

    [HttpGet]
    public IActionResult GetAllPlaces()
    {
        var places = _placeRepository.GetAllPlaces();
        return Ok(places);
    }

    [HttpGet("{id}")]
    public IActionResult GetPlaceById(int id)
    {
        var place = _placeRepository.GetPlaceById(id);
        if (place == null)
        {
            return NotFound();
        }
        return Ok(place);
    }

    [HttpPost]
    public IActionResult AddPlace([FromBody] Place place)
    {
        var addedPlace = _placeRepository.AddPlace(place);
        return CreatedAtAction(nameof(GetPlaceById), new { id = addedPlace.Id }, addedPlace);
    }

    [HttpPut("{id}")]
    public IActionResult UpdatePlace(int id, [FromBody] Place place)
    {
        if (id != place.Id)
        {
            return BadRequest();
        }

        var existingPlace = _placeRepository.GetPlaceById(id);
        if (existingPlace == null)
        {
            return NotFound();
        }

        _placeRepository.UpdatePlace(place);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeletePlace(int id)
    {
        var existingPlace = _placeRepository.GetPlaceById(id);
        if (existingPlace == null)
        {
            return NotFound();
        }

        _placeRepository.DeletePlace(id);

        return NoContent();
    }
}