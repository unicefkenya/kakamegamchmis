using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MCHMIS.Models;
using X.PagedList;

namespace MCHMIS.ViewModels
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public string RecaptchaKey { get; set; }
    }

    public class ListUserViewModel
    {
        public ApplicationUser User { get; set; }
        public IPagedList<ApplicationUser> Users { get; set; }
        public IList<UserRoleViewModel> Roles { get; set; }

        [DisplayName("Health Facility")]
        public int? HealthFacilityId { get; set; }

        public int? Page { get; set; }
        public int? PageSize { get; set; }

        [DisplayName("Any One Name")]
        public string Name { get; set; }

        public string Role { get; set; }
        public string Email { get; set; }
    }

    public class UserRoleViewModel
    {
        public string UserId { get; set; }
        public string Role { get; set; }
    }

    public class RegisterViewModel : CreateFields
    {
        [Display(Name = "Role"), Required]
        public string Role { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password (Leave blank for the password to be auto generated.)")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "First Name"), Required]
        public string FirstName { get; set; }

        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }

        [Display(Name = "Surname"), Required]
        public string Surname { get; set; }

        [Display(Name = "Phone Number"), Required]
        [StringLength(10, ErrorMessage = "The {0} must be at 10 characters long", MinimumLength = 10)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "The {0} must be numeric")]
        public string PhoneNumber { get; set; }

        [Display(Name = "National ID Number"), Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "The {0} must be numeric")]
        public string IdNumber { get; set; }

        [Display(Name = "Health Facility")]
        public int? HealthFacilityId { get; set; }

        public HealthFacility HealthFacility { get; set; }

        [Display(Name = "Sub County")]
        public int? SubCountyId { get; set; }

        public SubCounty SubCounty { get; set; }
    }

    public class UpdateProfileViewModel : CreateFields
    {
        public string Id { get; set; }

        [Display(Name = "Role")]
        public string Role { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password (Leave blank for the password to be auto generated.)")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "First Name"), Required]
        public string FirstName { get; set; }

        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }

        [Display(Name = "Surname"), Required]
        public string Surname { get; set; }

        [Display(Name = "Phone Number"), Required]
        public string PhoneNumber { get; set; }

        [Display(Name = "National ID Number"), Required]
        public string IdNumber { get; set; }

        public DateTime? LockoutEnd { get; set; }
        public string IsActive { get; set; }

        [Display(Name = "Health Facility")]
        public int? HealthFacilityId { get; set; }

        public HealthFacility HealthFacility { get; set; }

        [Display(Name = "Sub County")]
        public int? SubCountyId { get; set; }

        public SubCounty SubCounty { get; set; }

        [Display(Name = "Reset Password?")]
        public bool ResetPassword { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ChangePasswordForcedViewModel
    {
        public string Id { get; set; }

        [DataType(DataType.Password), Required]
        [DisplayName("Current Password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [DataType(DataType.Password), Required]
        [DisplayName("Current Password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class RolesViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}