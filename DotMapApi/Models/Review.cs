using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotMapApi.Models;

public class Review
{
    [Key]
    public int Id { get; set; }
    public string Comment { get; set; }
    public int Rating { get; set; }
    public int User { get; set; }
    public int PlaceId { get; set; }
}