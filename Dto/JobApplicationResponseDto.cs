using System;

namespace UCCD_App.Dto;

public class JobApplicationResponseDto
{
    public int Id { get; set; }
    public int JobOpportunityId { get; set; }
    public string JobTitle { get; set; } = "";
    public string CompanyName { get; set; } = "";
    public int StudentId { get; set; }
    public string StudentFullName { get; set; } = "";
    public string StudentEmail { get; set; } = "";
    public string StudentFaculty { get; set; } = "";
    public string CvFilePath { get; set; } = ""; // الملف الفعلي اللي اتبعت للشركة
    public DateTime AppliedAt { get; set; }
}