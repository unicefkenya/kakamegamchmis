using System.Collections.Generic;

namespace MCHMIS.Extensions
{
    public static class Statics
    {
        public static List<int> UserProfile()
        {
            //ApplicationDbContext db = new ApplicationDbContext();
            //string userId = HttpContext.User.GetUserId();
            //string userRoleId = db.Roles.First(c => c.Users.Select(i => i.UserId).Contains(userId)).Id;
            //var userprofiles = db.RoleProfiles.Where(p => p.RoleId == userRoleId).Select(p => p.TaskId).ToList();
            // return userprofiles;
            return new List<int>();
        }

        public static List<string> UserProfileByName()
        {
            //ApplicationDbContext db = new ApplicationDbContext();
            //string userId = HttpContext.Current.User.GetUserId();
            //string userRoleId = db.Roles.First(c => c.Users.Select(i => i.UserId).Contains(userId)).Id;

            //var userprofiles = db.RoleProfiles.Where(p => p.RoleId == userRoleId)
            //    .Select(p => p.SystemTask.Parent.Name + ":" + p.SystemTask.Name).ToList();
            //return userprofiles;
            return new List<string>();
        }
    }
}