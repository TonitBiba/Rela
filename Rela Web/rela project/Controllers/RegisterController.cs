using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rela_project.Models.LogIn;
using Rela_project.Models.Register;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Rela_project.Controllers
{
    public class RegisterController : Controller
    {

        [HttpGet]
        [RequireHttps]
        public ActionResult Index()
        {
            Register register = new Register();
            return View(register);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireHttps]
        public async Task<ActionResult> Index(Register register, HttpPostedFileBase userProfileImage) {
            string SECRETKEY = ConfigurationManager.AppSettings["captchaSecret"];
            string userResponse = register.gRecaptchaResponse;
            var webClient = new WebClient();
            string verification = webClient.DownloadString(ConfigurationManager.AppSettings["captchaLink"] + "secret=" + SECRETKEY + "&response=" + userResponse);
            var verificationJson = JsonConvert.DeserializeObject<CaptchaResponse>(verification);
            bool rezultati = verificationJson.success;
            var httpClient = new HttpClient();
            var jsonCaptchaLogs = new Logs { action = verificationJson.action, challenge_ts = verificationJson.challenge_ts, score = verificationJson.score, time_accessed = DateTime.Now, ip = Request.UserHostAddress, hostname = Request.UserHostName, success = verificationJson.success, url = Request.Url.ToString() };
            var captchaContent = new StringContent(JsonConvert.SerializeObject(jsonCaptchaLogs), Encoding.ASCII, "application/json");
            await httpClient.PostAsync(ConfigurationManager.AppSettings["url"] + "api/logs/saveRecaptchaLogs", captchaContent);

            if (!rezultati)
            {
                ViewBag.RegisterError = "CaptchaError";
                return View();
            }

            if (!ModelState.IsValid)
            {
                return View(register);
            }
        
            try
            {
                byte[] bufferImage = null;
                if (userProfileImage != null)
                {
                    bufferImage = new byte[userProfileImage.ContentLength];
                    userProfileImage.InputStream.Read(bufferImage, 0, userProfileImage.ContentLength);
                }
                var client = new HttpClient();
                string imageAsString = null;
                if (bufferImage != null)
                    imageAsString = Convert.ToBase64String(bufferImage, 0, bufferImage.Length);
                register.BirthDate = DateTime.ParseExact(register.BirthDate, "yyyy-MM-dd", null).ToString();

                register.user_image = imageAsString;
                var jsonContent = JsonConvert.SerializeObject(register);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await client.PostAsync( ConfigurationManager.AppSettings["url"] + "api/account/Register", content);
                var responseString = await response.Content.ReadAsStringAsync();
                TempData["LogInError"] = "checkEmail";
                return RedirectToAction("Index","LogIn");
            }
            catch (Exception ex)
            {
                ViewBag.RegisterError = "Error Sign up";
                return View();
            }
        }

        [HttpGet]
        public async Task<string> checkUsername(string UserName) {
            await saveLog();
            var httpClient = new HttpClient();
            var jsonContent = JsonConvert.SerializeObject(UserName);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(ConfigurationManager.AppSettings["url"] + "api/account/checkUsername", content);
            var responseString = await response.Content.ReadAsStringAsync();
            return responseString;
        }

        [HttpGet]
        public async Task<string> checkEmail(string Email) {
            await saveLog();
            var httpClient = new HttpClient();
            var jsonContent = JsonConvert.SerializeObject(Email);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(ConfigurationManager.AppSettings["url"] + "api/account/checkEmail", content);
            var result = await response.Content.ReadAsStringAsync();
            return result;
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