namespace UCCD_App.Dto
{
    public class UpdateCourseDto
    {
        public string Name { get; set; } = "";
        public int Capacity { get; set; }
        public decimal CertificationFee { get; set; }
        public int Duration { get; set; }
        public decimal Price { get; set; }
        public DateTime StartDate { get; set; }
        public string Type { get; set; } = "";
    }
}
