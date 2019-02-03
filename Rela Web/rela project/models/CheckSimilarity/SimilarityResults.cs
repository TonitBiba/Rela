using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rela_project.Models.CheckSimilarity
{
    public class SimilarityResults
    {
        public bool IsIdentical { get; set; }
        public float Confidence { get; set; }
    }
}