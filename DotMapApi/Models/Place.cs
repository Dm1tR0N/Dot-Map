using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DotMapApi.Models;

namespace DotMapApi.Models;

public class Place
{
    [Key]
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public List<Review>? Reviews { get; set; }

    public double Latitude { get; set; } 
    public double Longitude { get; set; }

    public byte[] Photo { get; set; }
}