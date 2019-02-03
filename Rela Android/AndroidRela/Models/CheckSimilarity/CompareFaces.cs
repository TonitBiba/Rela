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

namespace AndroidRela.Models.CheckSimilarity
{
    public class CompareFaces
    {
        public string userId { get; set; }

        public string fatherPhoto { get; set; }

        public string motherPhoto { get; set; }

        public string childrenPhoto { get; set; }
    }

    public class CompareFacesFamily
    {

        public string userId { get; set; }

        public string familyPhoto { get; set; }
    }
}