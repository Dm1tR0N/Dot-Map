using System.ComponentModel.DataAnnotations;

namespace DotMapApi.Models;

public class Place
{
    [Key]
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public List<Review>? Reviews { get; set; }
    public Location? Coordinates { get; set; }
    public byte[]? Photo { get; set; }
}