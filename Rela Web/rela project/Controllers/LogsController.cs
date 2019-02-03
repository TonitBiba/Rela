using Rela_project.Models;
using Rela_project.Models.LogIn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Rela_project.Controllers
{
    [RoutePrefix("api/Account")]
    public class LogsController : ApiController
    {
        private RelaEntities rela = new RelaEntities();
        [HttpPost]
        public async Task<IHttpActionResult> saveRecaptchaLogs(Logs logs)
        {
            try
            {
                rela.Logs.Add(new Log
                {
                    action = logs.action,
                    time_accessed = logs.time_accessed,
                    challenge_ts = logs.challenge_ts,
                    hostname = logs.hostname,
                    ip = logs.ip,
                    score = Convert.ToDecimal(logs.score),
                    url = logs.url,
                    success = logs.success
                });
                await rela.SaveChangesAsync();
            }
            catch (Exception ex) { }
            return Ok();
        }
    }
}
