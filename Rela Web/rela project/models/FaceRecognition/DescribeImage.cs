using Google.Cloud.Vision.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rela_project.Models.FaceRecognition
{
    public class DescribeImage
    {
        public string voiceBase64 { get; set; }
        public IReadOnlyList<FaceAnnotation> googleFace { get; set; }
        public IReadOnlyList<EntityAnnotation> label { get; set; }
        public IReadOnlyList<EntityAnnotation> landmark { get; set; }
        public IReadOnlyList<EntityAnnotation> logo { get; set; }
        public List<CognitiveMicrosoft> microsoftFace { get; set; }
    }
}