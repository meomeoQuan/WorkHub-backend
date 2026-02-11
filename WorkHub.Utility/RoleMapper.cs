using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkHub.Utility
{
    public static class RoleMapper
    {
        public static int MapRoleToRoleNumber(string role)
        {
            return role switch
            {
                "Admin" => 0,
                "User" => 1,
                _ => throw new ArgumentException("Invalid role", nameof(role)),
            };
        }
        public static string MapRoleNumberToRoleString(int role)
        {
            return role switch
            {
                0 => SD.Role_Admin,
                1 => SD.Role_User,
                _ => throw new ArgumentException("Invalid table name", nameof(role)),
            };
        }
    }
}
