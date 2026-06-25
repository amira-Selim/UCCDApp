using System.Collections.Generic;
using System.Threading.Tasks;
using UCCD_App.Dto;

namespace UCCD_App.Services;

public interface IVolunteerService
{
    // === ألوية الأدمن (Opportunities) ===
    Task<ApiResponse<VolunteerOpportunityResponseDto>> CreateOpportunityAsync(CreateVolunteerOpportunityDto dto);
    Task<ApiResponse<VolunteerOpportunityResponseDto>> UpdateOpportunityAsync(int id, CreateVolunteerOpportunityDto dto);
    Task<ApiResponse<IEnumerable<VolunteerOpportunityResponseDto>>> GetAllOpportunitiesAsync(bool? isActive);
    Task<ApiResponse<VolunteerOpportunityResponseDto>> GetOpportunityByIdAsync(int id);
    Task<ApiResponse<IEnumerable<VolunteerApplicationResponseDto>>> GetApplicationsByOpportunityIdAsync(int opportunityId);
    Task<ApiResponse<IEnumerable<VolunteerApplicationResponseDto>>> GetApplicationsByStudentIdAsync(int studentId);

    // === ألوية الطلاب (Applications) ===
    Task<ApiResponse<VolunteerApplicationResponseDto>> ApplyForOpportunityAsync(string studentEmail, int opportunityId, ApplyVolunteerDto dto);
    Task<ApiResponse<IEnumerable<VolunteerApplicationResponseDto>>> GetStudentApplicationsAsync(string studentEmail);
    // === تحكم الأدمن في القبول والرفض ===
    Task<ApiResponse<VolunteerApplicationResponseDto>> UpdateApplicationStatusAsync(UpdateVolunteerStatusDto dto);

    Task<ApiResponse<string>> CancelApplicationAsync(string email, int applicationId);
}