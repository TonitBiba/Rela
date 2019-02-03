using Newtonsoft.Json;
using Rela_project.Models;
using Rela_project.Models.CheckSimilarity;
using Rela_project.Models.Home;
using Rela_project.Models.LogIn;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Rela_project.Controllers
{
    public class CheckSimilarityController : Controller
    {
        [HttpGet]
        [RequireHttps]
        public async Task<ActionResult> CheckResultView() {
            await saveLog();
            if (Session.Count == 0)
            {
                TempData["LogInError"] = "Session has ended. Please log in again.";
                return RedirectToAction("Index", "LogIn");
            }
            ViewBag.ChekcSimilarityError = TempData["ChekcSimilarityError"];
            return View();
        }

        [HttpPost]
        [RequireHttps]
        public async Task<ActionResult> DisplayResults3View(Similarity3Persons similarity3Persons)
        {
            string SECRETKEY = ConfigurationManager.AppSettings["captchaSecret"];
            string userResponse = similarity3Persons.gRecaptchaResponse;
            var webClient = new WebClient();
            string verification = webClient.DownloadString(ConfigurationManager.AppSettings["captchaLink"] + "secret=" + SECRETKEY + "&response=" + userResponse);
            var verificationJson = JsonConvert.DeserializeObject<CaptchaResponse>(verification);
            bool rezultati = verificationJson.success;
            var httpClient = new HttpClient();
            var jsonCaptchaLogs = new Logs { action = verificationJson.action, challenge_ts = verificationJson.challenge_ts, score = verificationJson.score, time_accessed = DateTime.Now, ip = Request.UserHostAddress, hostname = Request.UserHostName, success = verificationJson.success, url = Request.Url.ToString() };
            var captchaContent = new StringContent(JsonConvert.SerializeObject(jsonCaptchaLogs), Encoding.ASCII, "application/json");
            await httpClient.PostAsync(ConfigurationManager.AppSettings["url"] + "api/logs/saveRecaptchaLogs", captchaContent);

            if (Session.Count == 0)
            {
                TempData["LogInError"] = "Session has ended. Please log in again.";
                return RedirectToAction("Index", "LogIn");
            }

            try
            {
                User user = (User)Session["userSession"];
                byte[] motherByte = new byte[similarity3Persons.mother.ContentLength];
                await similarity3Persons.mother.InputStream.ReadAsync(motherByte, 0, similarity3Persons.mother.ContentLength);
                string motherToBase64 = Convert.ToBase64String(motherByte, 0, motherByte.Length);

                byte[] fatherByte = new byte[similarity3Persons.father.ContentLength];
                await similarity3Persons.father.InputStream.ReadAsync(fatherByte, 0, similarity3Persons.father.ContentLength);
                string fatherToBase64 = Convert.ToBase64String(fatherByte, 0, fatherByte.Length);

                byte[] childrenByte = new byte[similarity3Persons.children.ContentLength];
                await similarity3Persons.children.InputStream.ReadAsync(childrenByte, 0, similarity3Persons.children.ContentLength);
                string childrenToBase64 = Convert.ToBase64String(childrenByte, 0, childrenByte.Length);

                if (Session["checkSimilarity3"] != null)
                {
                    var SessionData = (DisplayResultsModel)Session["checkSimilarity3"];
                    if (SessionData.father == fatherToBase64 && SessionData.mother == motherToBase64 && SessionData.children == childrenToBase64)
                        return View(SessionData);
                }

                var compareFaces = new CompareFaces { userId = user.userId, fatherPhoto = fatherToBase64, motherPhoto = motherToBase64, childrenPhoto = childrenToBase64 };

                var content = new StringContent(JsonConvert.SerializeObject(compareFaces), Encoding.UTF8, "application/json");

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Session["Access_Token"].ToString());

                var responseApi = await httpClient.PostAsync(ConfigurationManager.AppSettings["url"] + "api/FaceRecognition/CheckSimilarityResults", content);
                var result = await responseApi.Content.ReadAsStringAsync();

                if (!responseApi.IsSuccessStatusCode)
                {
                    var mesage = JsonConvert.DeserializeObject<Error>(result);
                    TempData["errorCheckSimilarity"] = mesage.Message;
                    return RedirectToAction("CheckResultView");
                }

                var checkSimilarity = JsonConvert.DeserializeObject<CheckSimilarity>(result);
                var displayResultsModel = new DisplayResultsModel { checkSimilarity = checkSimilarity, father = fatherToBase64, mother = motherToBase64, children = childrenToBase64 };
                Session["checkSimilarity3"] = displayResultsModel;
                return View(displayResultsModel);
            }
            catch (Exception ex)
            {
                TempData["errorCheckSimilarity"] = ex.ToString();
                return RedirectToAction("CheckResultView");
            }
        }

        [HttpPost]
        [RequireHttps]
        public async Task<ActionResult> DisplayResultsTogetherView(HttpPostedFileBase familyTogether) {
            await saveLog();
            if (Session.Count == 0)
            {
                TempData["LogInError"] = "Session has ended. Please log in again.";
                return RedirectToAction("Index", "LogIn");
            }
            var httpClient = new HttpClient();
                try
                {
                    User user = (User)Session["userSession"];
                    byte[] familyTogetherByte = new byte[familyTogether.ContentLength];
                    await familyTogether.InputStream.ReadAsync(familyTogetherByte, 0, familyTogether.ContentLength);
                    string familyToBase64 = Convert.ToBase64String(familyTogetherByte, 0, familyTogetherByte.Length);

                if (Session["FamilyCheckResult"] != null) {
                    var SessionData = (DisplayResultModelFamily)Session["FamilyCheckResult"];
                    if (SessionData.familyPhoto == familyToBase64)
                        return View(SessionData);
                }

                    var compareFamily = new CompareFacesFamily { userId = user.userId, familyPhoto = familyToBase64 };

                    var content = new StringContent(JsonConvert.SerializeObject(compareFamily), Encoding.UTF8, "application/json");

                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Session["Access_Token"].ToString());

                    var responseApi = await httpClient.PostAsync(ConfigurationManager.AppSettings["url"] + "api/FaceRecognition/FamilyCheck", content);
                    var result = await responseApi.Content.ReadAsStringAsync();

                var errorMessage = JsonConvert.DeserializeObject<Error>(result);
                if(errorMessage.Message != null)
                {
                    TempData["ChekcSimilarityError"] = errorMessage.Message;
                    return RedirectToAction("CheckResultView");
                }
                    

                if (responseApi.StatusCode == (HttpStatusCode)429) {
                    TempData["ChekcSimilarityError"] = "429";
                    return RedirectToAction("CheckResultView");
                }


                    var checkSimilarityTogether = JsonConvert.DeserializeObject<CheckSimilarityTogether>(result);

                    Session["FamilyCheckResult"] = new DisplayResultModelFamily {checkSimilarityTogether = checkSimilarityTogether, familyPhoto = familyToBase64 };

                    return View(new DisplayResultModelFamily { checkSimilarityTogether = checkSimilarityTogether, familyPhoto = familyToBase64 });
                }
                catch (Exception ex)
                {
                    return RedirectToAction("CheckResultView");
                }
        }

        [HttpGet]
        [RequireHttps]
        public async Task<ActionResult> ImagesProcesedHistory() {
            await saveLog();
            if (Session.Count == 0)
            {
                TempData["LogInError"] = "Session has ended. Please log in again.";
                return RedirectToAction("Index", "LogIn");
            }

            User user = (User)Session["userSession"];

            var httpClient = new HttpClient();
            var content = new StringContent(JsonConvert.SerializeObject(new { userId = user.userId}),Encoding.UTF8, "application/json");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Session["Access_Token"].ToString());
            var response = await httpClient.PostAsync(ConfigurationManager.AppSettings["url"] + "api/History/GetImages", content);
            var result = await response.Content.ReadAsStringAsync();
            var listOfImages = JsonConvert.DeserializeObject<List<HistoryCheckSimilarity>>(result);
            return View(listOfImages);
        }

        [HttpPost]
        [RequireHttps]
        public async Task<ActionResult> findFatherMotherChildren(HttpPostedFileBase familyTogether)
        {
            await saveLog();
            if (Session.Count == 0)
            {
                TempData["LogInError"] = "Session has ended. Please log in again.";
                return RedirectToAction("Index", "LogIn");
            }
            var httpClient = new HttpClient();
            try
            {
                User user = (User)Session["userSession"];
                byte[] familyTogetherByte = new byte[familyTogether.ContentLength];
                await familyTogether.InputStream.ReadAsync(familyTogetherByte, 0, familyTogether.ContentLength);
                string familyToBase64 = Convert.ToBase64String(familyTogetherByte, 0, familyTogetherByte.Length);
                if (Session["FamilyCheckResult"] != null)
                {
                    var SessionData = (DisplayResultModelFamily)Session["FamilyCheckResult"];
                    if (SessionData.familyPhoto == familyToBase64)
                    return View(SessionData);
                }
                var compareFamily = new CompareFacesFamily { userId = user.userId, familyPhoto = familyToBase64 };
                var content = new StringContent(JsonConvert.SerializeObject(compareFamily), Encoding.UTF8, "application/json");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Session["Access_Token"].ToString());
                var responseApi = await httpClient.PostAsync(ConfigurationManager.AppSettings["url"] + "api/FaceRecognition/FindFacesFamily", content);
                 var result = await responseApi.Content.ReadAsStringAsync();

                try
                {
                    var errorMessage = JsonConvert.DeserializeObject<Error>(result);
                    if (errorMessage.Message != null)
                    {
                        TempData["ChekcSimilarityError"] = errorMessage.Message;
                        return RedirectToAction("CheckResultView");
                    }
                }catch(Exception ex){}

                if (responseApi.StatusCode == (HttpStatusCode)429){
                    TempData["ChekcSimilarityError"] = "429";
                    return RedirectToAction("CheckResultView");
                }

                var cognitiveMicrosofts = JsonConvert.DeserializeObject<List<CognitiveMicrosoft>>(result);

                var checkSimilarityTogether = new CheckSimilarityTogether();
                checkSimilarityTogether.cognitiveMicrosoft = cognitiveMicrosofts;
                try
                {
                    var father = cognitiveMicrosofts.Where(face => face.faceAttributes.gender.Equals("male")).OrderByDescending(face => face.faceAttributes.age).ToList()[0];
                    var mother = cognitiveMicrosofts.Where(face => face.faceAttributes.gender.Equals("female")).OrderByDescending(face => face.faceAttributes.age).ToList()[0];
                    var children = cognitiveMicrosofts.OrderBy(face => face.faceAttributes.age).ToList()[0];

                    checkSimilarityTogether.fatherPositon = cognitiveMicrosofts.ToList().IndexOf(father);
                    checkSimilarityTogether.motherPosition = cognitiveMicrosofts.ToList().IndexOf(mother);
                    checkSimilarityTogether.childrenPosition = cognitiveMicrosofts.ToList().IndexOf(children);
                }catch(Exception ex) { }
                Session["FamilyData"] = new DisplayResultModelFamily { checkSimilarityTogether = checkSimilarityTogether, familyPhoto = familyToBase64 };
                return View(new DisplayResultModelFamily { checkSimilarityTogether = checkSimilarityTogether, familyPhoto = familyToBase64 });
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPost]
        public async Task<ActionResult> updateDiv(Guid father, Guid mother, Guid children)
        {
            await saveLog();
            try
            {
                if (Session.Count == 0)
                {
                    TempData["LogInError"] = "Session has ended. Please log in again.";
                    return RedirectToAction("Index", "LogIn");
                }

                var httpClient = new HttpClient();
                User user = (User)Session["userSession"];

                var content = new StringContent(JsonConvert.SerializeObject(new { userId = user.userId, father =father, mother = mother, children = children }), Encoding.UTF8, "application/json");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Session["Access_Token"].ToString());
                var responseApi = httpClient.PostAsync(ConfigurationManager.AppSettings["url"] + "api/FaceRecognition/Similarity", content).Result;
                var result = responseApi.Content.ReadAsStringAsync().Result;
                var similarityResults = JsonConvert.DeserializeObject<List<SimilarityResults>>(result);

                var checkResult = (DisplayResultModelFamily)Session["FamilyData"];
                checkResult.checkSimilarityTogether.similarityTest = similarityResults;


                return PartialView("_updateDiv", checkResult.checkSimilarityTogether);
            }
            catch (Exception ex)
            {
                return null;
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