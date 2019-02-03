using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rela_project.Models.LogIn
{
    public class TokenResponseApi
    {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public string expires_in { get; set; }
            public string userName { get; set; }
            public string issued { get; set; }
            public string expires { get; set; }
    }
}