using System.ComponentModel.DataAnnotations;

namespace DotMapApi.Models;

public class Location
{
    
    [Key]
    public int LocationId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}