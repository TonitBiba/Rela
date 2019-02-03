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

namespace AndroidRela.Models.Voice
{
    public class VoiceHistory
    {
        public int imageId { get; set; }

        public DateTime date { get; set; }

        public string image { get; set; }

        public int voiceId { get; set; }

        public string GoogleVoice { get; set; }
    }
}