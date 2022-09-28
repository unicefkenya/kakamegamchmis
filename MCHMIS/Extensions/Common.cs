using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MCHMIS.Extensions
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// User ID
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static string GetUserId(this ClaimsPrincipal user)
        {
            if (!user.Identity.IsAuthenticated)
                return null;

            ClaimsPrincipal currentUser = user;
            return currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
        }
        public static SelectList GetPager(this Controller controller, int? pageSize)
        {
            return new SelectList(new List<FilterParameter> {
                new FilterParameter { Id = 20, Name = "20" },
                new FilterParameter { Id = 50, Name = "50" },
                new FilterParameter { Id = 100, Name = "100"},
                new FilterParameter { Id = 1000, Name = "1000"},
                new FilterParameter { Id = 0, Name = "All"  },

            }, "Id", "Name", pageSize);
        }
    }
    public class FilterParameter
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}