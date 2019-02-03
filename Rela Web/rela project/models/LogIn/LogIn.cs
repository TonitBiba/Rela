using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Rela_project.Models.LogIn
{
    public class LogIn
    {
        [Display(Name = "Username or Email")]
        [Required(ErrorMessage = "Username or Email must be filled. ")]
        [StringLength(50,MinimumLength = 2,ErrorMessage = "Username or Email is not valid.")]
        public string username { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "You must type password to continue. ")]
        [StringLength(50,MinimumLength = 5,ErrorMessage ="Password is not valid.")]
        public string password { get; set; }

        [Display(Name = "Remember me?")]
        public bool rememberMe { get; set; }

        [Required(ErrorMessage ="Confirm that you are not robot?")]
        public string gRecaptchaResponse { get; set; }

    }
}