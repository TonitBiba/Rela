using Google.Cloud.TextToSpeech.V1;
using Google.Cloud.Vision.V1;
using Rela_project.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Rela_project.Services
{
    public class AnalyzeImage
    {
        public IReadOnlyList<EntityAnnotation> responseForLabels { get; set; }
        public IReadOnlyList<FaceAnnotation> responseFaceGoogle { get; set; }
        public IReadOnlyList<EntityAnnotation> responseLandMarkGoogle { get; set; }
        public IReadOnlyList<EntityAnnotation> responseForLogoGoogle { get; set; }
        public List<CognitiveMicrosoft> responseForFacesMicrosft { get; set; }

        private string textDescribeImage = "Welcome to Rela project. ";

        private FaceDescription faceDescription = new FaceDescription();

        public string describeImageWithVoice() {
            string base64Voice = generateVoice(generateTextForImage().Result);
            return base64Voice;
        }

        public async Task<string> generateTextForImage() {
            if (responseFaceGoogle.Count > 0)
            {
                int nFaces = responseFaceGoogle.Count;
                string faceDetected = nFaces == 1 ? "There is only one face detected. Lets describe it for you. " : "There are " + nFaces + " faces detected. Lets describe them for you.";
                for (int i = 0; i < responseForFacesMicrosft.Count; i++)
                {
                    string heOrShe = responseForFacesMicrosft[i].faceAttributes.gender == "male" ? "he" : "she";
                    string glasses = responseForFacesMicrosft[i].faceAttributes.glasses == "NoGlasses" ? " does not use glasses." : " use glasses.";
                    faceDetected += " The " + convertNumberToText(i + 1) + " person it is " + responseForFacesMicrosft[i].faceAttributes.gender + ", should be " + responseForFacesMicrosft[i].faceAttributes.age + " years old and " + glasses + ". ";
                    faceDetected += heOrShe + " looks " + determineEmotion(responseForFacesMicrosft[i].faceAttributes.emotion) + " and her hair color are " + determineColorHair(responseForFacesMicrosft[i].faceAttributes.hair) + ". ";

                    if (heOrShe == "he")
                        faceDetected += facialHairDetected(responseForFacesMicrosft[i].faceAttributes.facialHair);
                    else
                        faceDetected += makeupDetected(responseForFacesMicrosft[i].faceAttributes.makeup);
                    faceDetected += Occluded(responseForFacesMicrosft[i].faceAttributes.occlusion);
                }
                textDescribeImage += faceDetected;
            }

            if (responseLandMarkGoogle.Count > 0)
            {
                int nLandMark = responseLandMarkGoogle.Count;
                string landMarkDesc = nLandMark == 1 ? " There is only one landmark detected. Lets describe it for you." : " There are " + nLandMark + " landmarks detected. Lets describe them for you.";
                if (nLandMark == 1)
                    landMarkDesc += " Landmark should be " + responseLandMarkGoogle[0].Description + ".";
                else
                {
                    for (int i = 0; i < nLandMark; i++)
                    {
                        landMarkDesc += " The " + convertNumberToText(i + 1) + " landmark should be " + responseLandMarkGoogle[i].Description + ".";
                    }
                }
                textDescribeImage += landMarkDesc;
            }

            if (responseForLogoGoogle.Count > 0)
            {
                int nLogos = responseForLogoGoogle.Count;
                string logosDetected = nLogos == 1 ? " There is only one logo detected. Lets describe it." : " There are " + nLogos + " logos detected.";
                if (nLogos == 1)
                {
                    logosDetected += " Logo detected should be " + responseForLogoGoogle[0].Description + ".";
                }
                else
                {
                    for (int i = 0; i < nLogos; i++)
                    {
                        logosDetected += " The " + convertNumberToText(i + 1) + " should be " + responseForLogoGoogle[i].Description + ".";
                    }
                }

                textDescribeImage += logosDetected;
            }

            if (responseForLabels.Count > 0)
            {
                string labelDetected = "";
                if (responseFaceGoogle.Count == 0 && responseLandMarkGoogle.Count == 0 && responseForLogoGoogle.Count == 0)
                {
                    labelDetected = "We could not detect faces, landmarks or logos in your picture, but we can say a little about key thinks that we have detected. Let's mention them: ";
                    foreach (var label in responseForLabels)
                    {
                        labelDetected += label.Description + ", ";
                    }
                }
                else
                {
                    labelDetected = " At the end let us summarize keywords about your image: ";
                    foreach (var label in responseForLabels)
                    {
                        labelDetected += label.Description + ", ";
                    }
                }
                textDescribeImage += labelDetected;
            }
            return textDescribeImage;
        }

        public string determineEmotion(Emotion emotion)
        {
            Dictionary<string, float> emotionsUser = new Dictionary<string, float> {
                {"Anger",emotion.anger },
                {"Contempt",emotion.contempt },
                {"Disgust",emotion.disgust },
                {"Fear",emotion.fear },
                {"Happiness",emotion.happiness },
                {"Sadness",emotion.sadness },
                {"Surprise",emotion.surprise },
                {"Neutral",emotion.neutral }
            };
            string userEmotion = emotionsUser.OrderByDescending(u => u.Value).FirstOrDefault().Key;
            return userEmotion;
        }

        public string determineColorHair(Hair hair)
        {
            Dictionary<string, float> userHairColor = new Dictionary<string, float>();
            foreach (var userHair in hair.hairColor)
                userHairColor.Add(userHair.color, userHair.confidence);
            return userHairColor.OrderByDescending(u => u.Value).FirstOrDefault().Key;
        }

        public string facialHairDetected(Facialhair facialHair)
        {
            if (facialHair.beard > 0 && facialHair.moustache > 0 && facialHair.sideburns > 0) return " He has beard, moustache and sideburn.";

            else if (facialHair.beard > 0.1 && facialHair.moustache > 0.1 && facialHair.sideburns == 0) return " He has beard and moustache.";
            else if (facialHair.beard > 0.1 && facialHair.moustache == 0 && facialHair.sideburns > 0.1) return " He has beard and sideburns.";
            else if (facialHair.beard == 0 && facialHair.moustache > 0.1 && facialHair.sideburns > 0.1) return " He has moustache and sideburns.";

            else if (facialHair.beard > 0.1 && facialHair.moustache == 0 && facialHair.sideburns == 0) return " He has beard.";
            else if (facialHair.beard == 0 && facialHair.moustache > 0.1 && facialHair.sideburns == 0) return " He has moustache.";
            else if (facialHair.beard == 0 && facialHair.moustache == 0 && facialHair.sideburns > 0.1) return " He has sideburns.";

            else return " He has no facial hair.";
        }

        public string makeupDetected(Makeup makeup)
        {
            if (makeup.eyeMakeup && makeup.lipMakeup) return " She has made makeup in her eyes and lips.";
            else if (makeup.eyeMakeup && makeup.lipMakeup == false) return " She has made makeup in her eyes.";
            else if (makeup.eyeMakeup && makeup.lipMakeup == false) return " She has made makeup in her lips.";
            else return " She has not made makeup she looks so natyral.";
        }

        public string Occluded(Occlusion occlusion)
        {
            if (occlusion.eyeOccluded && occlusion.foreheadOccluded && occlusion.mouthOccluded) return " Also we have detected that eye, forehead and mouth are occluded.";

            else if (occlusion.eyeOccluded && occlusion.foreheadOccluded && occlusion.mouthOccluded == false) return " Also we have detected that eyes and forehead are occluded.";
            else if (occlusion.eyeOccluded && occlusion.foreheadOccluded == false && occlusion.mouthOccluded) return " Also we have detected that eyes and mouth are occluded.";
            else if (occlusion.eyeOccluded == false && occlusion.foreheadOccluded && occlusion.mouthOccluded) return " Also we have detected that forehead and mouth are occluded.";

            else if (occlusion.eyeOccluded && occlusion.foreheadOccluded == false && occlusion.mouthOccluded == false) return " Also we have detected that eyes are occluded.";
            else if (occlusion.eyeOccluded == false && occlusion.foreheadOccluded && occlusion.mouthOccluded == false) return " Also we have detected that forehead is occluded.";
            else if (occlusion.eyeOccluded == false && occlusion.foreheadOccluded == false && occlusion.mouthOccluded == false) return " Also we have detected that mouth is occluded.";

            else return "";
        }

        public string convertNumberToText(int number)
        {
            switch (number)
            {
                case 1:
                    return "first";
                case 2:
                    return "second";
                case 3:
                    return "third";
                case 4:
                    return "fourth";
                case 5:
                    return "fifth";
                case 6:
                    return "sixth";
                case 7:
                    return "seventh";
                case 8:
                    return "eighth";
                case 9:
                    return "ninth";
                case 10:
                    return "tenth";
                case 11:
                    return "eleventh";
            }
            return number.ToString();
        }

        public string generateVoice(string text)
        {
            var client = TextToSpeechClient.Create();
            var inputText = new SynthesisInput
            {
                Text = text + " Thank you for using Rela."
            };
            var voiceParameters = new VoiceSelectionParams
            {
                LanguageCode = "en-US",
                SsmlGender = SsmlVoiceGender.Female
            };

            var audioParams = new AudioConfig
            {
                AudioEncoding = AudioEncoding.Mp3
            };

            var response = client.SynthesizeSpeech(inputText, voiceParameters, audioParams);
            string audioContent = response.AudioContent.ToBase64();

            return audioContent;
        }
    }
}