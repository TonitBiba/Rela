using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Rela_project.Models.Register
{
    public class Register
    {
        [Display(Name = "Email")]
        [StringLength(50, MinimumLength = 1)]
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage ="This is not an valid mail.")]
        public string Email { get; set; }

        [Display(Name = "Password")]
        [StringLength(20, MinimumLength = 6)]
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }

        [Display(Name = "Confirm password")]
        [StringLength(25, MinimumLength = 6)]
        [Required(ErrorMessage = "You must confirm password.")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public byte Level { get; set; }

        [Display(Name = "Firstname")]
        [StringLength(100, MinimumLength = 1)]
        [Required(ErrorMessage = "Type firstname.")]
        public string FirstName { get; set; }

        [Display(Name = "Lastname")]
        [StringLength(100, MinimumLength = 1)]
        [Required(ErrorMessage = "Type lastname.")]
        public string LastName { get; set; }

        [Display(Name = "Image")]
        public string user_image { get; set; }

        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Username")]
        [StringLength(50, MinimumLength = 4)]
        [Required(ErrorMessage = "Username is required.")]
        public string UserName { get; set; }

        [Display(Name = "Birthday")]
        [Required(ErrorMessage = "Choose your birthday.")]
        public string BirthDate { get; set; }

        [Display(Name = "Gender")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Confirm that you are not robot?")]
        public string gRecaptchaResponse { get; set; }

    }
}