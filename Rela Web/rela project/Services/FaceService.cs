using Newtonsoft.Json;
using Rela_project.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace Rela_project.Services
{
    public class FaceDescription
    {
        public string ImagePath { get; set; }
        private const string SUBSCRIPTIONKEY = "Key";
        private const string BASEURI = "WindowsApiUrl";

        public async Task<string> MakeAnalysisRequestAsync()
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", SUBSCRIPTIONKEY);
            string requestParameters = "returnFaceId=true&returnFaceLandmarks=false&returnFaceAttributes=age,gender,headPose,smile,facialHair,glasses,emotion,hair,makeup,occlusion,accessories,blur,exposure,noise";
            string uri = BASEURI + "?" + requestParameters;
            HttpResponseMessage httpResponseMessage;
            byte[] byteImage = getImageAsByteArray(ImagePath);
            ByteArrayContent byteArrayContent = new ByteArrayContent(byteImage);
            byteArrayContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            httpResponseMessage = await httpClient.PostAsync(uri, byteArrayContent);
            string responseFromMicrosoft = await httpResponseMessage.Content.ReadAsStringAsync();
            return responseFromMicrosoft;
        }

        public async Task<List<CognitiveMicrosoft>> MakeAnalysisRequestAsync(byte[] imageByte)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", SUBSCRIPTIONKEY);
            string requestParameters = "returnFaceId=true&returnFaceLandmarks=false&returnFaceAttributes=age,gender,headPose,smile,facialHair,glasses,emotion,hair,makeup,occlusion,accessories,blur,exposure,noise";
            string uri = BASEURI + "?" + requestParameters;
            HttpResponseMessage httpResponseMessage;
            byte[] byteImage = imageByte;
            ByteArrayContent byteArrayContent = new ByteArrayContent(byteImage);
            byteArrayContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            httpResponseMessage = await httpClient.PostAsync(uri, byteArrayContent);
            var responseFromMicrosoft = await httpResponseMessage.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<CognitiveMicrosoft>>(responseFromMicrosoft);
        }

        private byte[] getImageAsByteArray(string imagePath)
        {
            FileStream fileStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }
    }
}