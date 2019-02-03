using Google.Cloud.Vision.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rela_project.Models
{
    public class DescribeImageVoice
    {
        public string voiceBase64 { get; set; }
        public string imageBase64 { get; set; }
        public List<CognitiveMicrosoft> microsoftFace { get; set; }
        public IReadOnlyList<FaceAnnotation> googleFace { get; set; }
        public IReadOnlyList<EntityAnnotation> label { get; set; }
        public IReadOnlyList<EntityAnnotation> landmark { get; set; }
        public IReadOnlyList<EntityAnnotation> logo { get; set; }
    }

    public class VoiceHistory
    {
        public int imageId { get; set; }

        public DateTime date { get; set; }

        public string image { get; set; }

        public int voiceId { get; set; }

        public string GoogleVoice { get; set; }
    }
}