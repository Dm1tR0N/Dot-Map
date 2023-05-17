using System.Collections.Generic;
using System.Linq;
using DotMapApi.Interface;
using DotMapApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DotMapApi.Interface.Classes;

public class PlaceRepository : IPlaceRepository
{
    private readonly ApplicationDbContext _context;

    public PlaceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Place> GetAllPlaces()
    {
        var result = _context.Places.ToList();
        return result;
    }

    public Place GetPlaceById(int id)
    {
        return _context.Places.SingleOrDefault(p => p.Id == id);
    }

    public Place AddPlace(Place place)
    {
        _context.Places.Add(place);
        _context.SaveChanges();
        return place;
    }

    public void UpdatePlace(Place place)
    {
        _context.Entry(place).State = EntityState.Modified;
        _context.SaveChanges();
    }

    public void DeletePlace(int id)
    {
        var place = _context.Places.FirstOrDefault(p => p.Id == id);
        if (place != null)
        {
            _context.Places.Remove(place);
            _context.SaveChanges();
        }
    }
}