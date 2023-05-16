using DotMapApi.Models;

namespace DotMapApi.Interface;

public interface IPlaceRepository
{
    IEnumerable<Place> GetAllPlaces();
    Place GetPlaceById(int id);
    Place AddPlace(Place place);
    void UpdatePlace(Place place);
    void DeletePlace(int id);
}