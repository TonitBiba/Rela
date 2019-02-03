using Google.Cloud.Vision.V1;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.ProjectOxford.Face;
using Newtonsoft.Json;
using Rela_project.Models;
using Rela_project.Models.CheckSimilarity;
using Rela_project.Models.FaceRecognition;
using Rela_project.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Image = Google.Cloud.Vision.V1.Image;

namespace Rela_project.Controllers
{
    [Authorize(Roles ="User")]
    [RoutePrefix("api/FaceRecognition")]
    public class FaceRecognitionController : ApiController
    {
        private readonly FaceServiceClient faceServiceClient = new FaceServiceClient(ConfigurationManager.AppSettings["SUBSCRIPTIONKEY"], ConfigurationManager.AppSettings["MicrosoftApi"]);

        private ApplicationUserManager _userManager;
        private RelaEntities rela = new RelaEntities();
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [HttpPost]
        [Route("CheckSimilarityResults")]
        public async Task<IHttpActionResult> checkSimiliarityResults(CompareFaces compareFaces) {
            try
            {
                var user = await UserManager.FindByIdAsync(compareFaces.userId);

                var faceDescription = new FaceDescription();
                var father = await faceDescription.MakeAnalysisRequestAsync(Convert.FromBase64String(compareFaces.fatherPhoto));
                if (father.Count == 0)
                    return BadRequest("No face detected to father picture.");
                var fatherReal = father.OrderByDescending(t => t.faceAttributes.age).ToList()[0];

                var mother = await faceDescription.MakeAnalysisRequestAsync(Convert.FromBase64String(compareFaces.motherPhoto));
                if (mother.Count == 0)
                    return BadRequest("No face detected to mother picture.");
                var motherReal = mother.OrderByDescending(t => t.faceAttributes.age).ToList()[0];

                var children = await faceDescription.MakeAnalysisRequestAsync(Convert.FromBase64String(compareFaces.childrenPhoto));
                if (mother.Count == 0)
                    return BadRequest("No face detected to children picture.");
                var childrenReal = children.OrderBy(t => t.faceAttributes.age).ToList()[0];

                var responseForFather = await faceServiceClient.VerifyAsync(fatherReal.faceId, childrenReal.faceId);
                var responseForMother = await faceServiceClient.VerifyAsync(motherReal.faceId, childrenReal.faceId);

                List<SimilarityTestC> listSimilarityTest = new List<SimilarityTestC> {
                    new SimilarityTestC{Confidence = responseForFather.Confidence, IsIdentical = responseForFather.IsIdentical},
                    new SimilarityTestC{Confidence = responseForMother.Confidence, IsIdentical = responseForMother.IsIdentical}
                };

                rela.ImagesProceseds.Add(new ImagesProcesed{ image = compareFaces.childrenPhoto, date = DateTime.Now, UserId = user.Id });
                await rela.SaveChangesAsync();
                int imageId = rela.ImagesProceseds.OrderByDescending(img => img.date).ToList()[0].imageId;

                rela.MicrosoftFaces.Add(new MicrosoftFace { imageId = imageId, MicrosoftFace1 = JsonConvert.SerializeObject(childrenReal) });
                imageId = rela.ImagesProceseds.OrderByDescending(img => img.date).ToList()[0].imageId;
                rela.MicrosoftFaces.Add(new MicrosoftFace { imageId = imageId, MicrosoftFace1 = JsonConvert.SerializeObject(fatherReal) });
                imageId = rela.ImagesProceseds.OrderByDescending(img => img.date).ToList()[0].imageId;
                rela.MicrosoftFaces.Add(new MicrosoftFace { imageId = imageId, MicrosoftFace1 = JsonConvert.SerializeObject(motherReal) });

                rela.SimilarityTests.Add(new SimilarityTest {
                    father = JsonConvert.SerializeObject(responseForFather),
                    mother = JsonConvert.SerializeObject(responseForFather),
                    imageId = imageId});

                await rela.SaveChangesAsync();
                
                return Ok(new ResponseForSimilarityTest { similarityTest = listSimilarityTest, cognitiveMicrosoft = new List<CognitiveMicrosoft> { fatherReal, motherReal, childrenReal } });
            }
            catch (Exception ex) {
                return BadRequest(ex.ToString());
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> FindFacesFamily(CompareFacesFamily compareFacesFamily) {
            try
            {
                var faceDescription = new FaceDescription();
                var user = await UserManager.FindByIdAsync(compareFacesFamily.userID);
                var resultsOfFacesDetected = await faceDescription.MakeAnalysisRequestAsync(Convert.FromBase64String(compareFacesFamily.familyPhoto));
                if (resultsOfFacesDetected.Count < 3)
                    return BadRequest(resultsOfFacesDetected.Count.ToString());
                
                return Ok(resultsOfFacesDetected);
            }
            catch (Exception ex) {
                return BadRequest("Error");
            }
        }

        [HttpPost]
        [Route("Similarity")]
        public async Task<IHttpActionResult> CheckSimilarityFamily(CompareFamily compareFamily) {
            var responseForFather = await faceServiceClient.VerifyAsync(compareFamily.father, compareFamily.children);
            var responseForMother = await faceServiceClient.VerifyAsync(compareFamily.mother, compareFamily.children);

            List<SimilarityTestC> listSimilarityTest = new List<SimilarityTestC> {
                    new SimilarityTestC{Confidence = responseForFather.Confidence, IsIdentical = responseForFather.IsIdentical},
                    new SimilarityTestC{Confidence = responseForMother.Confidence, IsIdentical = responseForMother.IsIdentical}
                };

            return Ok(listSimilarityTest);
        }

        [HttpPost]
        [Route("DescribeImageWithVoice")]
        public async Task<IHttpActionResult> describeImageWithVoice([FromBody]ImageToVoice imageToVoice) {
            try
            {
                var user = await UserManager.FindByIdAsync(imageToVoice.userId);

                var faceDescription = new FaceDescription();
                var googleClient = await ImageAnnotatorClient.CreateAsync();
                var byteImage = Convert.FromBase64String(imageToVoice.base64Image);
                var image = Image.FromBytes(byteImage);

                var responseForFacesGoogle = await googleClient.DetectFacesAsync(image);
                var responseForLabels = await googleClient.DetectLabelsAsync(image);
                var responseForLandmark = await googleClient.DetectLandmarksAsync(image);
                var responseForLogo = await googleClient.DetectLogosAsync(image);

                var analyzeImage = new AnalyzeImage();
                analyzeImage.responseFaceGoogle = responseForFacesGoogle;
                analyzeImage.responseForLabels = responseForLabels;
                analyzeImage.responseForLogoGoogle = responseForLogo;
                analyzeImage.responseLandMarkGoogle = responseForLandmark;

                var responseFormMicrosoftFace = new List<CognitiveMicrosoft>();
                if (responseForFacesGoogle.Count  > 0) {
                    responseFormMicrosoftFace = await faceDescription.MakeAnalysisRequestAsync(byteImage);
                    analyzeImage.responseForFacesMicrosft = responseFormMicrosoftFace;
                }

                string base64Voice = analyzeImage.describeImageWithVoice();

                rela.ImagesProceseds.Add(new ImagesProcesed { UserId = user.Id, date = DateTime.Now, image = imageToVoice.base64Image });
                await rela.SaveChangesAsync();
                int imageId = rela.ImagesProceseds.OrderByDescending(img => img.date).ToList()[0].imageId;

                if (responseForFacesGoogle.Count>0)
                    rela.GoogleFaces.Add(new GoogleFace { GoogleFace1 = JsonConvert.SerializeObject(responseForFacesGoogle), imageId = imageId });

                if(responseForLabels.Count>0)
                    rela.GoogleLabels.Add(new GoogleLabel { GoogleLabel1= JsonConvert.SerializeObject(responseForLabels), imageId = imageId });

                if(responseForLandmark.Count>0)
                    rela.GoogleLandmarks.Add(new GoogleLandmark { GoogleLandamark= JsonConvert.SerializeObject(responseForLandmark), imageId = imageId});

                if(responseForLogo.Count>0)
                    rela.GoogleLogoes.Add(new GoogleLogo { GoogleLogo1= JsonConvert.SerializeObject(responseForLogo), imageId = imageId });

                if (responseFormMicrosoftFace.Count > 0)
                    rela.MicrosoftFaces.Add(new MicrosoftFace { imageId = imageId, MicrosoftFace1= JsonConvert.SerializeObject(responseFormMicrosoftFace)});

                rela.Voices.Add(new Voice { imageId = imageId, GoogleVoice = base64Voice});
                
                await rela.SaveChangesAsync();
                
                DescribeImage describeImage = new DescribeImage()
                {
                    googleFace = responseForFacesGoogle,
                    label = responseForLabels,
                    landmark = responseForLandmark,
                    logo = responseForLogo,
                    voiceBase64 = base64Voice,
                    microsoftFace = responseFormMicrosoftFace
                };

                return Ok(describeImage);
            }
            catch (Exception ex) {
                return BadRequest("Error");
            }
        }
    }
}
