using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rela_project.Models.Home
{
    public class Similarity3Persons
    {
        public HttpPostedFileBase father{ get; set; }
        public HttpPostedFileBase mother { get; set; }
        public HttpPostedFileBase children{ get; set; }
        public string gRecaptchaResponse { get; set; }
    }
}