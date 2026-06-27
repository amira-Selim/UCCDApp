using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UCCD_App.Dto;
using UCCD_App.Models;

namespace UCCD_App.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class CompaniesController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly UCCD_App.Services.IEmailService _emailService;

    public CompaniesController(UserManager<ApplicationUser> userManager, UCCD_App.Services.IEmailService emailService)
    {
        _userManager = userManager;
        _emailService = emailService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCompanies()
    {
        var users = await _userManager.GetUsersInRoleAsync("Company");
        var result = users.Select(u => new
        {
            Id = u.Id,
            Name = u.FullName,
            Email = u.Email
        });

        return Ok(new ApiResponse<object> { Success = true, Data = result });
    }

    [HttpPost]
    public async Task<IActionResult> CreateCompany([FromBody] CreateCompanyDto dto)
    {
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
        {
            return BadRequest(new ApiResponse<object> { Success = false, Message = "Company email already exists." });
        }

        var companyUser = new ApplicationUser
        {
            FullName = dto.Name,
            Email = dto.Email,
            UserName = dto.Email,
            RequirePasswordChange = true
        };

        var result = await _userManager.CreateAsync(companyUser, dto.Password);
        if (!result.Succeeded)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = string.Join(", ", result.Errors.Select(e => e.Description))
            });
        }

        await _userManager.AddToRoleAsync(companyUser, "Company");

        // Send email to the company
        string subject = "Welcome to UCCD Portal - Company Account";
        string body = $@"
            <h3>Welcome to UCCD Portal, {dto.Name}!</h3>
            <p>Your company account has been created successfully by the Administrator.</p>
            <p>Here are your login credentials:</p>
            <ul>
                <li><strong>Email:</strong> {dto.Email}</li>
                <li><strong>Temporary Password:</strong> {dto.Password}</li>
            </ul>
            <p>You will be required to change your password upon your first login.</p>
            <br>
            <p>Thank you,</p>
            <p>UCCD Admin Team</p>";

        try
        {
            await _emailService.SendEmailToUserAsync(dto.Email, subject, body);
        }
        catch (System.Exception)
        {
            // Log or ignore email failure so we don't break the creation process
        }

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Company created successfully.",
            Data = new { Id = companyUser.Id, Name = companyUser.FullName, Email = companyUser.Email }
        });
    }
}
