using System.Collections.Generic;

namespace WorkHub.Models.DTOs.ModelDTOs
{
    public class DashboardStatsDTO
    {
        public long TotalRevenue { get; set; }
        public int TotalUsers { get; set; }
        public int TotalJobs { get; set; }
        public int TotalPremiumUsers { get; set; }
        public List<DailyRevenueDTO> RevenueChartData { get; set; } = new();
        public double RevenueGrowthPercentage { get; set; }
        public int UserGrowthCount { get; set; }
        public int JobGrowthCount { get; set; }
        public double PremiumGrowthPercentage { get; set; }
    }

    public class DailyRevenueDTO
    {
        public string Day { get; set; } = null!;
        public long Revenue { get; set; }
    }
}
