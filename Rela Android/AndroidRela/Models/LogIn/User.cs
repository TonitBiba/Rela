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

namespace AndroidRela.Models.LogIn
{
    public class User
    {
        public string FirstName { get; set; }
        public string userId { get; set; }
        public string LastName { get; set; }
        public string user_image { get; set; }
    }
}