using Microsoft.EntityFrameworkCore;
using UCCD_App.Context;
using UCCD_App.Models;

namespace UCCD_App.Repo
{
    public class WishlistRepo : IWishlistRepo
    {
        private readonly AppDbContext _context;

        public WishlistRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Wishlist wishlist)
        {
            await _context.Wishlists.AddAsync(wishlist);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Wishlist>> GetByStudentIdAsync(string studentId)
        {
            return await _context.Wishlists
                .Include(w => w.Course)
                .Where(w => w.StudentId == studentId)
                .ToListAsync();
        }

        public async Task<Wishlist?> GetByStudentAndCourseAsync(string studentId, int courseId)
        {
            return await _context.Wishlists
                .Include(w => w.Course) // ضروري عشان الـ Service تشوف بيانات الكورس
                .FirstOrDefaultAsync(w => w.StudentId == studentId && w.CourseId == courseId);
        }

        public async Task DeleteAsync(Wishlist wishlist)
        {
            _context.Wishlists.Remove(wishlist);
            await _context.SaveChangesAsync();
        }
    }
}