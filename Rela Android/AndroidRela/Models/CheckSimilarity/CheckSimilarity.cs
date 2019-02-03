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
    public class CheckSimilarity
    {
        public List<SimilarityResults> similarityTest { get; set; }

        public List<CognitiveMicrosoft> cognitiveMicrosoft { get; set; }
    }

    public class CheckSimilarityTogether
    {

        public List<SimilarityResults> similarityTest { get; set; }

        public List<CognitiveMicrosoft> cognitiveMicrosoft { get; set; }

        public int fatherPositon { get; set; }

        public int motherPosition { get; set; }

        public int childrenPosition { get; set; }
    }
}