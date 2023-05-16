using System.ComponentModel.DataAnnotations;

namespace DotMapApi.Models;

public class Review
{
    [Key]
    public int Id { get; set; }
    public string Comment { get; set; }
    public int Rating { get; set; }
    public User? User { get; set; }
}