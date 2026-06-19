using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UCCD_App.Dto;
using UCCD_App.Services;

namespace UCCD_App.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;
        public WishlistController(IWishlistService wishlistService) => _wishlistService = wishlistService;

        // ميثود مساعدة لجلب الـ ID بشكل أنظف
        private string? CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] WishlistDto dto)
        {
            if (CurrentUserId == null) return Unauthorized();

            var result = await _wishlistService.AddToWishlistAsync(CurrentUserId, dto);
            
            if (result == null)
                return Conflict(new { message = "Item already exists in wishlist" });

            return Ok(new { message = "Added successfully", data = result });
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            if (CurrentUserId == null) return Unauthorized();
            
            var result = await _wishlistService.GetStudentWishlistAsync(CurrentUserId);
            return Ok(result);
        }

        [HttpDelete("{courseId}")]
        public async Task<IActionResult> Remove(int courseId)
        {
            if (CurrentUserId == null) return Unauthorized();

            var removed = await _wishlistService.RemoveFromWishlistAsync(CurrentUserId, courseId);
            if (!removed) return NotFound(new { message = "Item not found" });

            return Ok(new { message = "Removed successfully" });
        }
    }
}