using Microsoft.AspNetCore.Http;

namespace UCCD_App.Dto;

public class ApplyJobDto
{
    public IFormFile CvFile { get; set; } = null!;
    public string? CoverLetter { get; set; }
}