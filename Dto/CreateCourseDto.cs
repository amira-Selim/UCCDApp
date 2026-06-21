using System.ComponentModel.DataAnnotations;

namespace UCCD_App.Dto;

public class CreateCourseDto
{
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
    [RegularExpression("(WorksShop|Training|Advising)")]
    public string Type { get; set; } = "";
    public string? Instructor { get; set; }
}
