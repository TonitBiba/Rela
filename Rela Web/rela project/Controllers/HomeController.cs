using Newtonsoft.Json;
using Rela_project.Models;
using Rela_project.Models.LogIn;
using Rela_project.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Rela_project.Controllers
{ 
    public class HomeController : Controller
    {       
        public async Task<ActionResult> AccountConfirmed() {
            await saveLog();
            return View();
        }

        //HttpGet
        public async Task<ActionResult> ResetPassword(string email,string code) {
            await saveLog();
            ViewBag.Email = email;
            ViewBag.Code = code;
            return View();
        }

        //HttpGet
        public async Task<ActionResult> ResetPasswordConfirmed() {
            await saveLog();
            return View();
        }

        //Home/Main
        [HttpGet]
        [RequireHttps]
        public async Task<ActionResult> Main()
        {
            await saveLog();
            if (Session.Count == 0)
            {
                TempData["LogInError"] = "Session has ended. Please log in again.";
                return RedirectToAction("Index","LogIn");
            }
            else
            {
                return View();
            }
        }

        //Home.getUserInfo
        public async Task<ActionResult> getUserInfo()
        {
            await saveLog();
            var request = new HttpRequestMessage(HttpMethod.Get, ConfigurationManager.AppSettings["url"]  + "api/account/values");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Session["Access_Token"].ToString());
            var client = new HttpClient();
            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            return null;
        }

        public async Task<ActionResult> LogOff() {
            await saveLog();
            Session.RemoveAll();
            Session.Abandon();
            return RedirectToAction("Index", "LogIn");
        }

        public async Task saveLog()
        {
            var httpClient = new HttpClient();
            var jsonCaptchaLogs = new Logs { action = null, challenge_ts = null, score = 1, time_accessed = DateTime.Now, ip = Request.UserHostAddress, hostname = Request.UserHostName, success = true, url = Request.Url.ToString() };
            var captchaContent = new StringContent(JsonConvert.SerializeObject(jsonCaptchaLogs), Encoding.ASCII, "application/json");
            await httpClient.PostAsync(ConfigurationManager.AppSettings["url"] + "api/logs/saveRecaptchaLogs", captchaContent);
        }
    }
}
