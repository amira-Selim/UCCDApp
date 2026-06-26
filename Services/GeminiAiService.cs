using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using UCCD_App.Dto;

namespace UCCD_App.Services
{
    public class GeminiAiService : IAiService
    {
        private readonly HttpClient _httpClient;
        private readonly IProfileService _profileService;
        private readonly IJobBoardService _jobBoardService;
        private readonly ICourseService _courseService;
        private readonly IConfiguration _configuration;

        public GeminiAiService(HttpClient httpClient, IProfileService profileService, IJobBoardService jobBoardService, ICourseService courseService, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _profileService = profileService;
            _jobBoardService = jobBoardService;
            _courseService = courseService;
            _configuration = configuration;
        }

        private async Task<string> CallGeminiApiAsync(string prompt)
        {
            var apiKey = _configuration["Gemini:ApiKey"]?.Trim();
            if (string.IsNullOrEmpty(apiKey) || apiKey == "YOUR_GEMINI_API_KEY_HERE")
            {
                // Fallback for development if key is missing
                return "This is a placeholder AI response because the Gemini API Key is not set in appsettings.json.";
            }

            var requestUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-flash-latest:generateContent?key={apiKey}";

            var payload = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[] { new { text = prompt } }
                    }
                }
            };

            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(requestUrl, content);
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                return $"Error generating content. Status Code: {response.StatusCode}. Details: {errorBody}";
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(responseJson);
            
            try
            {
                var generatedText = doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text").GetString();

                return generatedText;
            }
            catch (Exception)
            {
                return "Error parsing AI response.";
            }
        }

        public async Task<AiResponseDto> GenerateCoverLetterAsync(string studentEmail, int jobId)
        {
            var profileResult = await _profileService.GetProfileDataAsync(studentEmail);
            var jobResult = await _jobBoardService.GetJobByIdAsync(jobId);

            if (!profileResult.Success || profileResult.Data == null)
            {
                return new AiResponseDto { ResultText = "Error: Student profile not found." };
            }

            if (!jobResult.Success || jobResult.Data == null)
            {
                return new AiResponseDto { ResultText = "Error: Job not found." };
            }

            var profile = profileResult.Data;
            var job = jobResult.Data;

            var prompt = $@"
Act as an expert career advisor. Write a professional cover letter for a student applying for a job.
Here are the student's details:
Name: {profile.FullName}
Faculty: {profile.Faculty}
Skills: {profile.Skills}
Experience/Bio: {profile.CareerGoal}

Here are the job details:
Job Title: {job.Title}
Company: {job.CompanyName}
Job Description: {job.Description}
Requirements: {job.Requirements}

Please write a concise, compelling cover letter (around 3 paragraphs) that highlights how the student's skills align with the job requirements. Use a professional tone. Return ONLY the cover letter text.
";

            var aiResult = await CallGeminiApiAsync(prompt);

            return new AiResponseDto { ResultText = aiResult };
        }

        public async Task<IEnumerable<JobOpportunityResponseDto>> GetRecommendedJobsAsync(string studentEmail)
        {
            var profileResult = await _profileService.GetProfileDataAsync(studentEmail);
            if (!profileResult.Success || profileResult.Data == null) return new List<JobOpportunityResponseDto>();

            var allJobsResult = await _jobBoardService.GetApprovedJobsForStudentsAsync(studentEmail, false);
            if (!allJobsResult.Success || allJobsResult.Data == null) return new List<JobOpportunityResponseDto>();

            var profile = profileResult.Data;
            var jobsList = allJobsResult.Data.ToList();

            if (!jobsList.Any()) return new List<JobOpportunityResponseDto>();

            // For simplicity and speed in this demo, if the API key is not set, we'll just return the first 3 jobs
            var apiKey = _configuration["Gemini:ApiKey"];
            if (string.IsNullOrEmpty(apiKey) || apiKey == "YOUR_GEMINI_API_KEY_HERE")
            {
                return jobsList.Take(3);
            }

            var jobsJson = JsonSerializer.Serialize(jobsList.Select(j => new { j.Id, j.Title, j.Requirements }));
            
            var prompt = $@"
You are an AI Job Matching Assistant.
Student Skills: {profile.Skills}
Student Faculty: {profile.Faculty}

Available Jobs (JSON format):
{jobsJson}

Based on the student's skills and faculty, return the IDs of the top 3 best matching jobs. 
Return ONLY a comma-separated list of IDs (e.g., 1, 5, 8). Do not include any other text.
";

            var aiResult = await CallGeminiApiAsync(prompt);

            var recommendedJobIds = new List<int>();
            var matches = System.Text.RegularExpressions.Regex.Matches(aiResult, @"\d+");
            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                if (int.TryParse(match.Value, out int id))
                {
                    recommendedJobIds.Add(id);
                }
            }

            var recommendedJobs = jobsList.Where(j => recommendedJobIds.Contains(j.Id)).ToList();
            
            // Fallback if AI parsing fails or API key is invalid
            if (!recommendedJobs.Any())
            {
                var fallbackJobs = jobsList.Where(j => 
                    j.Title.Contains(profile.Faculty, StringComparison.OrdinalIgnoreCase) ||
                    (profile.Skills != null && j.Title.Contains(profile.Skills.Split(',').FirstOrDefault() ?? "", StringComparison.OrdinalIgnoreCase))
                ).Take(3).ToList();

                if (fallbackJobs.Any()) return fallbackJobs;
                return jobsList.Take(3);
            }

            return recommendedJobs;
        }

        public async Task<IEnumerable<CourseResponseDto>> GetRecommendedCoursesAsync(string studentEmail, AiCourseRecommendationRequestDto request)
        {
            var profileResult = await _profileService.GetProfileDataAsync(studentEmail);
            if (!profileResult.Success || profileResult.Data == null) return new List<CourseResponseDto>();

            var allCourses = await _courseService.GetAllCoursesAsync();
            var coursesList = allCourses.ToList();

            if (!coursesList.Any()) return new List<CourseResponseDto>();

            var apiKey = _configuration["Gemini:ApiKey"]?.Trim();
            if (string.IsNullOrEmpty(apiKey) || apiKey == "YOUR_GEMINI_API_KEY_HERE")
            {
                return coursesList.Take(3);
            }

            var coursesJson = JsonSerializer.Serialize(coursesList.Select(c => new { c.Id, c.Name, c.Type }));
            var profile = profileResult.Data;
            
            // Build prompt combining profile skills and user's answers
            var prompt = $@"
You are an AI Course Advisor.
Student Skills: {profile.Skills}
Student Faculty: {profile.Faculty}

The student just answered a NEW questionnaire with the following:
1. Field of interest: {request?.FieldOfInterest ?? "Not specified"}
2. Primary career goal: {request?.CareerGoal ?? "Not specified"}
3. Current level in this field: {request?.CurrentLevel ?? "Not specified"}

Available Courses (JSON format):
{coursesJson}

CRITICAL: Your recommendations MUST be based PRIMARILY on the student's NEW questionnaire answers (Field of interest and Career goal). Do NOT just recommend courses based on their existing profile skills. Give priority to what they just typed in the questionnaire.

Based on this, return the IDs of the top 3 best matching courses from the list.
Make sure the course complexity matches their stated level if possible.
Return ONLY a comma-separated list of IDs (e.g., 2, 6, 9). Do not include any other text.
";

            var aiResult = await CallGeminiApiAsync(prompt);

            if (aiResult.StartsWith("Error"))
            {
                return new List<CourseResponseDto> { new CourseResponseDto { Id = -1, Name = aiResult, Type = "AI Error" } };
            }

            var recommendedCourseIds = new List<int>();
            var matches = System.Text.RegularExpressions.Regex.Matches(aiResult, @"\d+");
            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                if (int.TryParse(match.Value, out int id))
                {
                    recommendedCourseIds.Add(id);
                }
            }

            var recommendedCourses = coursesList.Where(c => recommendedCourseIds.Contains(c.Id)).ToList();
            
            if (!recommendedCourses.Any())
            {
                var fallbackStr = request?.FieldOfInterest ?? "";
                var fallbackCourses = coursesList.Where(c => 
                    c.Name.Contains(fallbackStr, StringComparison.OrdinalIgnoreCase) ||
                    (c.Type != null && c.Type.Contains(fallbackStr, StringComparison.OrdinalIgnoreCase))
                ).Take(3).ToList();

                if (fallbackCourses.Any()) return fallbackCourses;
                return coursesList.Take(3);
            }

            return recommendedCourses;
        }
    }
}
