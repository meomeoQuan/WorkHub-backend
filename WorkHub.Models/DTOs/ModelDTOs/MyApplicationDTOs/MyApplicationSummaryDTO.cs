namespace WorkHub.Models.DTOs.ModelDTOs.MyApplicationDTOs
{
    public class MyApplicationSummaryDTO
    {
        public int TotalApplications { get; set; }
        public int Pending { get; set; }
        public int UnderReview { get; set; }
        public int Accepted { get; set; }
        public int Rejected { get; set; }
    }
}
