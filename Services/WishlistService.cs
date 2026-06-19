using UCCD_App.Dto;
using UCCD_App.Models;
using UCCD_App.Repo;

namespace UCCD_App.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IWishlistRepo _wishlistRepo;

        public WishlistService(IWishlistRepo wishlistRepo)
        {
            _wishlistRepo = wishlistRepo;
        }

        public async Task<WishlistReturnDto?> AddToWishlistAsync(string studentId, WishlistDto dto)
        {
            // التأكد إذا كان موجود مسبقاً
            var existing = await _wishlistRepo.GetByStudentAndCourseAsync(studentId, dto.CourseId);
            if (existing != null) return null;

            var wishlist = new Wishlist
            {
                StudentId = studentId,
                CourseId = dto.CourseId,
                AddedDate = DateTime.UtcNow
            };

            await _wishlistRepo.AddAsync(wishlist);

            // جلب البيانات بعد الحفظ لضمان وجود بيانات الكورس (بسبب الـ Include في الـ Repo)
            var result = await _wishlistRepo.GetByStudentAndCourseAsync(studentId, dto.CourseId);

            return new WishlistReturnDto
            {
                Id = result!.Id,
                CourseId = result.CourseId,
                CourseName = result.Course?.Name ?? "Unknown",
                Price = result.Course?.Price ?? 0,
                AddedDate = result.AddedDate
            };
        }

        public async Task<List<WishlistReturnDto>> GetStudentWishlistAsync(string studentId)
        {
            var items = await _wishlistRepo.GetByStudentIdAsync(studentId);
            return items.Select(w => new WishlistReturnDto
            {
                Id = w.Id,
                CourseId = w.CourseId,
                CourseName = w.Course?.Name ?? "",
                Price = w.Course?.Price ?? 0,
                AddedDate = w.AddedDate
            }).ToList();
        }

        public async Task<bool> RemoveFromWishlistAsync(string studentId, int courseId)
        {
            var item = await _wishlistRepo.GetByStudentAndCourseAsync(studentId, courseId);
            if (item == null) return false;

            await _wishlistRepo.DeleteAsync(item);
            return true;
        }
    }
}