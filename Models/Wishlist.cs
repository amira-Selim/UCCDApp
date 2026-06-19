using System.ComponentModel.DataAnnotations.Schema;

namespace UCCD_App.Models
{
    public class Wishlist : BaseEntity
    {
        public string StudentId { get; set; }

        [ForeignKey(nameof(StudentId))]
        public ApplicationUser? Student { get; set; }

        public int CourseId { get; set; }

        [ForeignKey(nameof(CourseId))]
        public Course? Course { get; set; }

        public DateTime AddedDate { get; set; } = DateTime.UtcNow;
    }
}

