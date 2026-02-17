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


    }
}
