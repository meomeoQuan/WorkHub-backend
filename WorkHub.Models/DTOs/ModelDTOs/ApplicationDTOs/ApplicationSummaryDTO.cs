namespace WorkHub.Models.DTOs.ModelDTOs.ApplicationDTOs
{
    public class ApplicationSummaryDTO
    {
        public int TotalApplications { get; set; }
        public int New { get; set; }
        public int Reviewing { get; set; }
        public int Accepted { get; set; }
        public int Rejected { get; set; }
    }
}
