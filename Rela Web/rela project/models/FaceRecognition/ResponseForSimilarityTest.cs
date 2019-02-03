using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rela_project.Models.FaceRecognition
{
    public class ResponseForSimilarityTest
    {
        public List<SimilarityTestC> similarityTest { get; set; }
        public List<CognitiveMicrosoft> cognitiveMicrosoft{ get; set; }
    }

    public class ResponseForSimilarityTestFamily {
        public List<SimilarityTestC> similarityTest { get; set; }
        public List<CognitiveMicrosoft> cognitiveMicrosoft { get; set; }
        public int fatherPositon { get; set; }
        public int motherPosition { get; set; }
        public int childrenPosition { get; set; }
    }

}