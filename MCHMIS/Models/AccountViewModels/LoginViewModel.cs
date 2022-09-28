using System.ComponentModel.DataAnnotations;

namespace MCHMIS.Models.AccountViewModels
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm that you are not a Robot.")]
        [StringLength(4)]
        public string CaptchaCode { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public string RecaptchaKey { get; set; }

        public string RecaptchaResponse { get; set; }
    }
}