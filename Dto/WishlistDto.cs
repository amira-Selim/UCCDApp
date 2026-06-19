namespace UCCD_App.Dto
{
    // الداتا اللي بنستقبلها من الفرونت إند
    public class WishlistDto
    {
        public int CourseId { get; set; }
    }

    // الداتا اللي بنرجعها للفرونت إند (فيها معلومات الكورس كاملة)
    public class WishlistReturnDto
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime AddedDate { get; set; }
    }
}