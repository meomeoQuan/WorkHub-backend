using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkHub.Utility
{
    public static class SD
    {
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


        //============================ JOIN COLLECTION TABLE STRING ==================================

        public const string CollectionJoin_PostLikes = "PostLikes";
        public const string CollectionJoin_Comments = "Comments";
        public const string CollectionJoin_Applications = "Applications";
        public const string CollectionJoin_Posts = "Posts";
        public const string CollectionJoin_Recruitments = "Recruitments";
        public const string CollectionJoin_UserFollowFollowers = "UserFollowFollowers";
        public const string CollectionJoin_UserFollowFollowings = "UserFollowFollowings";
        public const string CollectionJoin_UserSchedules = "UserSchedules";
        public const string CollectionJoin_UserDetail = "UserDetail";
        public const string CollectionJoin_InverseParentComment = "InverseParentComment";


    }
}
