using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Http.Results;
using System.Web.Routing;

namespace Rela_project.Services
{
    public class HandleSession : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if(HttpContext.Current.Session["User"] == null)
            {
                var httpResponseMessage = actionExecutedContext.Request.CreateResponse(HttpStatusCode.Redirect);
                httpResponseMessage.Headers.Location = new Uri("Eul");
            }
        }
    }
}