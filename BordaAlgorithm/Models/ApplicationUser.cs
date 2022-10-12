using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BordaAlgorithm.Models
{
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

            // Add custom user claims here
            return userIdentity;
        }

        public string Email_ { get; set; }
        public string Password { get; set; }
        public string User_Status { get; set; }
        public long? Create_By { get; set; }
        public Nullable<System.DateTime> Create_Date { get; set; }
        public long? Edit_By { get; set; }
        public Nullable<System.DateTime> Edit_Date { get; set; }
    }
    public class SelectedItem
    {
        public String Value { get; set; }
        public String Text { get; set; }
    }

    public class ApplicationUserView
    {
        public ApplicationUser applicationUser { get; set; }
        public IEnumerable<string> selectedGroup { get; set; }
        public IEnumerable<SelectListItem> listGroup { get; set; }
        public Boolean isChange { get; set; }
        public String newPassword { get; set; }
    }
    public class IdentityModel
    {
    }
}