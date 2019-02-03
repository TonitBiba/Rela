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
using SQLite;

namespace AndroidRela.Models.SqlLite
{
    [Table("Users")]
    class UserSession
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int id { get; set; }

        public string FirstName { get; set; }

        public string userId { get; set; }

        public string LastName { get; set; }

        public string user_image { get; set; }

        public string Token { get; set; }
    }
}