using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rela_project.Models.CheckSimilarity
{
    public class CheckSimilarity
    {
        public List<SimilarityResults> similarityTest { get; set; }

        public List<CognitiveMicrosoft> cognitiveMicrosoft { get; set; }
    }

    public class CheckSimilarityTogether {

        public List<SimilarityResults> similarityTest { get; set; }

        public List<CognitiveMicrosoft> cognitiveMicrosoft { get; set; }

        public int fatherPositon { get; set; }

        public int motherPosition { get; set; }

        public int childrenPosition { get; set; }
    }

}