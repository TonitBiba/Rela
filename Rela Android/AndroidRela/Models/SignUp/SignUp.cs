using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace AndroidRela.Models.SignUp
{
    class SignUp
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public byte Level { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string user_image { get; set; }

        public string PhoneNumber { get; set; }

        public string UserName { get; set; }

        public string BirthDate { get; set; }

        public string Gender { get; set; }

        public string gRecaptchaResponse { get; set; }

    }
}