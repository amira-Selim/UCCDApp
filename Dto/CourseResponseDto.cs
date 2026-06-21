using System.ComponentModel.DataAnnotations;
using UCCD_App.Models;

namespace UCCD_App.Dto;

public class CourseResponseDto
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = "";
    public DateTime? StartDate { get; set; }
    [Required]
    public int Duration { get; set; }
    [Required]
    public int Capacity { get; set; }
    [Required]
    public decimal Price { get; set; }
    [Required]
    public decimal CertificationFee { get; set; }
    [Required]
    public string Type { get; set; } = "";
    public string? Instructor { get; set; }
}
