using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rela_project.Models.LogIn
{
    public class CaptchaResponse
    {
        public bool success { get; set;}
        public string challenge_ts { get; set;}
        public string hostname { get; set; }
        public float score { get; set; }
        public string action { get; set; }
    }
}