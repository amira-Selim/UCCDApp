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

    public CompaniesController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
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
            UserName = dto.Email.Split('@').First(),
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

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Company created successfully.",
            Data = new { Id = companyUser.Id, Name = companyUser.FullName, Email = companyUser.Email }
        });
    }
}
