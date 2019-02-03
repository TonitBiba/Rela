using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rela_project.Models.FaceRecognition
{
    public class CompareFaces
    {
        public string userId { get; set; }

        public string fatherPhoto { get; set; }

        public string motherPhoto { get; set; }

        public string childrenPhoto { get; set; }
    }

    public class CompareFacesFamily {
        public string userID { get; set; }

        public string familyPhoto { get; set; }

    }

}