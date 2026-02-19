namespace WorkHub.Models.DTOs.ModelDTOs.ApplicationDTOs
{
    public class ApplicationSummaryDTO
    {
        public int TotalApplications { get; set; }
        public int New { get; set; }
        public int Reviewing { get; set; }
        public int Shortlisted { get; set; }
        public int Interviewed { get; set; }
    }
}
