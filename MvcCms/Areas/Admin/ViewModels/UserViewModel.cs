using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcCms.Areas.Admin.ViewModels
{
    public class UserViewModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Display name")]
        public string DisplayName { get; set; }

        [Required]
        public string Email { get; set; }

        [Display(Name = "Current password")]
        public string CurrentPassword { get; set; }

        [System.ComponentModel.DataAnnotations.Compare("ConfirmPassword",
            ErrorMessage = "The new password and confirmation do not match.")]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [Display(Name = "Confirm password")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Role")]
        public string SelectedRole { get; set; }

        private readonly List<string> _roles = new List<string>();

        public IEnumerable<SelectListItem> Roles
        {
            get { return new SelectList(_roles); }
        }

        public void LoadUserRoles(IEnumerable<IdentityRole> roles)
        {
            _roles.AddRange(roles.Select(x => x.Name));
        }
    }
}