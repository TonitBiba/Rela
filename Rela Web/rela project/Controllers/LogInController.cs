using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rela_project.Models.LogIn;

namespace Rela_project.Controllers
{
    public class LogInController : Controller
    {
        [HttpGet]
        [RequireHttps]
        public async Task<ActionResult> Index()
        {
            LogIn login = new LogIn();
            ViewBag.LogInError = TempData["LogInError"];
            ViewBag.LockError = TempData["LockError"];
            await saveLog();
            return View(login);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireHttps]
        public async Task<ActionResult> Index(LogIn logIn)
        {
            string SECRETKEY = ConfigurationManager.AppSettings["captchaSecret"];
            string userResponse = logIn.gRecaptchaResponse;

            var webClient = new WebClient();

            string verification = webClient.DownloadString(ConfigurationManager.AppSettings["captchaLink"] + "secret=" + SECRETKEY + "&response=" + userResponse);

            var verificationJson = JsonConvert.DeserializeObject<CaptchaResponse>(verification);
            bool rezultati = verificationJson.success;

            var httpClient = new HttpClient();
            var jsonCaptchaLogs = new Logs {
                action = verificationJson.action,
                challenge_ts = verificationJson.challenge_ts,
                score = verificationJson.score,
                time_accessed = DateTime.Now,
                ip = Request.UserHostAddress,
                hostname = Request.UserHostName,
                success = verificationJson.success,
                url = Request.Url.ToString()};
            var captchaContent = new StringContent(JsonConvert.SerializeObject(jsonCaptchaLogs), Encoding.ASCII, "application/json");
            await httpClient.PostAsync(ConfigurationManager.AppSettings["url"] + "api/logs/saveRecaptchaLogs", captchaContent);

            if (!rezultati)
            {
                TempData["LogInError"] = "RecaptchaError";
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
            {
                return View(logIn);
            }
            try
            {
                var credentialsToBeSend = new List<KeyValuePair<string, string>> {
                    new KeyValuePair<string, string>("username",logIn.username),
                    new KeyValuePair<string, string>("password",logIn.password),
                    new KeyValuePair<string, string>("grant_type","password")
                };
                var request = new HttpRequestMessage(HttpMethod.Post, ConfigurationManager.AppSettings["url"]+"Token");
                request.Content = new FormUrlEncodedContent(credentialsToBeSend);

                var client = new HttpClient();
                var response = await client.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == (HttpStatusCode)429) {
                    TempData["LogInError"] = "429";
                    return RedirectToAction("Index");
                }

                LogInError deserializeContent = JsonConvert.DeserializeObject<LogInError>(content);

                if (deserializeContent.error != null)
                {
                    TempData["LogInError"] = deserializeContent.error;
                    TempData["LockError"] = deserializeContent.error_description;
                    return RedirectToAction("Index");
                }
                else
                {
                    TokenResponseApi tokenResponse = JsonConvert.DeserializeObject<TokenResponseApi>(content);
                    Session["Access_Token"] = tokenResponse.access_token;
                    string username = logIn.username;
                    var jsonContent = JsonConvert.SerializeObject(username);
                    var LogIncontent = new StringContent(jsonContent, Encoding.ASCII, "application/json");

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Session["Access_Token"].ToString());

                    response = await client.PostAsync(ConfigurationManager.AppSettings["url"]+ "api/Account/userdata", LogIncontent);
                    var userData = await response.Content.ReadAsStringAsync();

                    User user = JsonConvert.DeserializeObject<User>(userData);
                    user.user_image = "data:Image/png;base64," + user.user_image;
                    Session["userSession"] = user;
                    return RedirectToAction("Main", "Home");
                }
            }
            catch (Exception ex)
            {
                TempData["LogInError"] = ex;
                return RedirectToAction("Index", "LogIn");
            }
        }

        //Post
        //Home/ForgotPasword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<string> ForgotPassword(string email)
        {
            try
            {
                var httpClient = new HttpClient();
                await saveLog();
                var content = new StringContent(JsonConvert.SerializeObject(new { Email = email }), Encoding.UTF8, "application/json");
                var result = await httpClient.PostAsync(ConfigurationManager.AppSettings["url"] + "api/account/forgotpassword", content);
                var response = await result.Content.ReadAsStringAsync();
                if (result.IsSuccessStatusCode)
                {
                    return JsonConvert.SerializeObject(new { message = "Please check your email for further details." });
                }
                else
                {
                    return JsonConvert.SerializeObject(new { message = "This email does not exist in the system. Please check it again." });
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { message = "Error in server side" });
            }
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