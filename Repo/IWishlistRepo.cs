using UCCD_App.Models;

namespace UCCD_App.Repo
{
    public interface IWishlistRepo
    {
        Task AddAsync(Wishlist wishlist);
        Task<List<Wishlist>> GetByStudentIdAsync(string studentId);
        Task<Wishlist?> GetByStudentAndCourseAsync(string studentId, int courseId);
        Task DeleteAsync(Wishlist wishlist);
    }
}