using UCCD_App.Dto;

namespace UCCD_App.Services
{
    public interface IWishlistService
    {
        Task<WishlistReturnDto?> AddToWishlistAsync(string studentId, WishlistDto dto);
        Task<List<WishlistReturnDto>> GetStudentWishlistAsync(string studentId);
        Task<bool> RemoveFromWishlistAsync(string studentId, int courseId);
    }
}