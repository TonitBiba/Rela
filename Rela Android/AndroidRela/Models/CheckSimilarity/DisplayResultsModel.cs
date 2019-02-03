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
    public class DisplayResultsModel
    {
        public string father { get; set; }
        public string mother { get; set; }
        public string children { get; set; }
        public CheckSimilarity checkSimilarity { get; set; }
    }


    public class DisplayResultModelFamily
    {
        public string familyPhoto { get; set; }

        public CheckSimilarityTogether checkSimilarityTogether { get; set; }
    }
}