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
using Google.Cloud.Vision.V1;

namespace AndroidRela.Models.Voice
{
    class DescribeImageVoice
    {
        public string voiceBase64 { get; set; }
        public string imageBase64 { get; set; }
        public List<CognitiveMicrosoft> microsoftFace { get; set; }
        public IReadOnlyList<FaceAnnotation> googleFace { get; set; }
        public IReadOnlyList<EntityAnnotation> label { get; set; }
        public IReadOnlyList<EntityAnnotation> landmark { get; set; }
        public IReadOnlyList<EntityAnnotation> logo { get; set; }
    }
}