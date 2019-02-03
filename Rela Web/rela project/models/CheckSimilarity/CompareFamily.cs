using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rela_project.Models.CheckSimilarity
{
    public class CompareFamily
    {
        public string userId { get; set; }

        public Guid father { get; set; }

        public Guid mother { get; set; }

        public Guid children { get; set; }

    }
}