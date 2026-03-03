using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkHub.Utility
{
    public static class SD
    {


        // ===== Job Types =====
        public const string JobType_FullTime = "Full Time";
        public const string JobType_PartTime = "Part Time";
        public const string JobType_Freelance = "Freelance";
        public const string JobType_Seasonal = "Seasonal";

        // ===== Categories =====
        public const string Category_IT = "IT";
        public const string Category_Retail = "Retail";
        public const string Category_Education = "Education";

        //=================== ORDER STATUS ==================

        public const string OrderStatus_Pending = "Pending";
        public const string OrderStatus_Paid = "Paid";
        public const string OrderStatus_Canceled = "Canceled";


        //=================== ROLES ==================

        public const string Role_User= "User";
        public const string Role_Admin = "Admin";

        //=================== EXTERNAL AUTH PROVIDERS ==================

        public const string Provider_Google = "Google";
        public const string Provider_Facebook = "Facebook";

        //=================== JOIN TABLE STRINGS ==================

        public const string Join_UserDetail = "UserDetail";
        public const string Join_User = "User";
        public const string Join_Recruitment = "Recruitment";
        public const string Join_Post = "Post";
        public const string Join_Follower = "Follower";
        public const string Join_Following = "Following";
        public const string Join_ParentComment = "ParentComment";
        public const string Join_Subscription = "Subscription";


        //============================ JOIN COLLECTION TABLE STRING ==================================

        public const string Collection_Join_PostLikes = "PostLikes";
        public const string Collection_Join_Comments = "Comments";
        public const string Collection_Join_Applications = "Applications";
        public const string Collection_Join_Posts = "Posts";
        public const string Collection_Join_Recruitments = "Recruitments";
        public const string Collection_Join_UserFollowFollowers = "UserFollowFollowers";
        public const string Collection_Join_UserFollowFollowings = "UserFollowFollowings";
        public const string Collection_Join_UserSchedules = "UserSchedules";
        public const string Collection_Join_UserDetail = "UserDetail";
        public const string Collection_Join_InverseParentComment = "InverseParentComment";
        public const string Collection_Join_Orders = "Orders";


        //============================ PLAN NAMES ==================================
        public const string Plan_Free = "free";
        public const string Plan_Silver = "silver";
        public const string Plan_Gold = "gold";

        //============================ PLAN LIMITS ==================================
        public const int Free_Post_Limit = 5;
        public const int Free_Apply_Limit = 5;
        public const int Silver_Post_Limit = 20;

        //============================ HELPER METHODS ==================================
        public static DateTime CalculateCycleStart(DateTime? baseDate)
        {
            var now = DateTime.UtcNow;
            if (!baseDate.HasValue) return now.AddMonths(-1);

            // Calculate the start of the current cycle
            // Example: baseDate = Jan 15, now = Mar 10
            // Cycle 1: Jan 15 - Feb 15
            // Cycle 2: Feb 15 - Mar 15
            // Result for now (Mar 10) = Feb 15

            var cycleDate = baseDate.Value;
            while (cycleDate.AddMonths(1) <= now)
            {
                cycleDate = cycleDate.AddMonths(1);
            }
            return cycleDate;
        }

    }
}
