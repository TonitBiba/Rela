using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using AndroidRela.Adapters;
using AndroidRela.Models.CheckSimilarity;
using AndroidRela.Models.SqlLite;
using SQLite;

namespace AndroidRela.Fragments
{
    public class FamilyThreePhotoFragment : Fragment
    {
        public static readonly int imageFatherId = 1000, imageMotherId = 1001, imageChildrenId = 1002;
        private SQLiteConnection connection;
        private string dbPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "users.db3");
        private ImageView imgvFather, imgvMother, imgvChildren;
        private Android.Support.Design.Widget.FloatingActionButton btnFatherChoose, btnChooseMother, btnChooseChildren;
        private LinearLayout relativeLayout1, relativeLayout3, relativeLayout2;
        private Button btnCheckSimilarityAnalyze;
        private ListView fatherList;
        View view;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.FamilyThreePhoto, container, false);
            var metrics = Resources.DisplayMetrics;
            int width = metrics.WidthPixels;
            int height = metrics.HeightPixels;
            imgvFather = view.FindViewById<ImageView>(Resource.Id.imgvFather);
            imgvMother = view.FindViewById<ImageView>(Resource.Id.imgvMother);
            imgvChildren = view.FindViewById<ImageView>(Resource.Id.imgvChildren);
            btnFatherChoose = view.FindViewById<Android.Support.Design.Widget.FloatingActionButton>(Resource.Id.btnFatherChoose);
            btnChooseMother = view.FindViewById<Android.Support.Design.Widget.FloatingActionButton>(Resource.Id.btnChooseMother);
            btnChooseChildren = view.FindViewById<Android.Support.Design.Widget.FloatingActionButton>(Resource.Id.btnChooseChildren);
            relativeLayout1 = view.FindViewById<LinearLayout>(Resource.Id.relativeLayout1);
            relativeLayout2 = view.FindViewById<LinearLayout>(Resource.Id.relativeLayout2);
            relativeLayout3 = view.FindViewById<LinearLayout>(Resource.Id.relativeLayout3);
            btnCheckSimilarityAnalyze = view.FindViewById<Button>(Resource.Id.btnCheckSimilarityAnalyze);
            fatherList = view.FindViewById<ListView>(Resource.Id.fatherList);

            imgvFather.LayoutParameters.Width = width / 2 - 50;
            imgvFather.LayoutParameters.Height = width / 2 - 50;
            imgvMother.LayoutParameters.Width = width / 2 - 50;
            imgvMother.LayoutParameters.Height = width / 2 - 50;
            imgvChildren.LayoutParameters.Width = width / 2 - 50;
            imgvChildren.LayoutParameters.Height = width / 2 - 50;
            relativeLayout1.LayoutParameters.Width = width / 2 - 50;
            relativeLayout2.LayoutParameters.Width = width / 2 - 50;
            relativeLayout3.LayoutParameters.Width = width / 2 - 50;
            relativeLayout1.LayoutParameters.Height = width / 2 - 50;
            relativeLayout2.LayoutParameters.Height = width / 2 - 50;
            relativeLayout3.LayoutParameters.Height = width / 2 - 50;

            btnFatherChoose.Click += HandleFatherChoose;
            btnChooseMother.Click += HandleMotherChoose;
            btnChooseChildren.Click += HandleChildrenChoose;
            btnCheckSimilarityAnalyze.Click += HandleBtnCheckSimilarityAnalysis;

            return view;
        }

        private async void HandleBtnCheckSimilarityAnalysis(object sender, EventArgs e)
        {
            var progressDialog = new ProgressDialog(view.Context);
            progressDialog.SetCancelable(true);
            progressDialog.SetMessage("Please wait!");
            progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
            progressDialog.Show();
            try
            {
                var httpClient = new HttpClient();
                connection = new SQLiteConnection(dbPath);
                var userData = connection.Get<UserSession>(1);
                string fatherToBase64 = await convertImageToBase64(imgvFather);
                string motherToBase64 = await convertImageToBase64(imgvMother);
                string childrenToBase64 = await convertImageToBase64(imgvChildren);
                var compareFaces = new CompareFaces
                {
                    userId = userData.userId,
                    fatherPhoto = fatherToBase64,
                    motherPhoto = motherToBase64,
                    childrenPhoto = childrenToBase64
                };
                var content = new StringContent(JsonConvert.SerializeObject(compareFaces), Encoding.UTF8, "application/json");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userData.Token);

                var responseApi = await httpClient.PostAsync("UrlApi" + "api/FaceRecognition/CheckSimilarityResults", content);
                var result = await responseApi.Content.ReadAsStringAsync();

                var checkSimilarity = JsonConvert.DeserializeObject<CheckSimilarity>(result);
                var displayResultsModel = new DisplayResultsModel { checkSimilarity = checkSimilarity, father = fatherToBase64, mother = motherToBase64, children = childrenToBase64 };
                List<string> attributes = new List<string> {
                    "Contempt", "Disgust","Anger", "Fear","Happiness","Neutral","Sadness","Smile","Surprise","Similarity","Glasses"
                };

                List<string> fatherResults = new List<string> {
                     displayResultsModel.checkSimilarity.cognitiveMicrosoft[0].faceAttributes.emotion.contempt.ToString(),
                     displayResultsModel.checkSimilarity.cognitiveMicrosoft[0].faceAttributes.emotion.disgust.ToString(),
                     displayResultsModel.checkSimilarity.cognitiveMicrosoft[0].faceAttributes.emotion.anger.ToString(),
                     displayResultsModel.checkSimilarity.cognitiveMicrosoft[0].faceAttributes.emotion.fear.ToString(),
                     displayResultsModel.checkSimilarity.cognitiveMicrosoft[0].faceAttributes.emotion.happiness.ToString(),
                     displayResultsModel.checkSimilarity.cognitiveMicrosoft[0].faceAttributes.emotion.neutral.ToString(),
                     displayResultsModel.checkSimilarity.cognitiveMicrosoft[0].faceAttributes.emotion.sadness.ToString(),
                     displayResultsModel.checkSimilarity.cognitiveMicrosoft[0].faceAttributes.smile.ToString(),
                     displayResultsModel.checkSimilarity.cognitiveMicrosoft[0].faceAttributes.emotion.surprise.ToString(),
                     displayResultsModel.checkSimilarity.similarityTest[0].Confidence.ToString(),
                     displayResultsModel.checkSimilarity.cognitiveMicrosoft[0].faceAttributes.glasses.ToString()
                };

                List<string> motherResults = new List<string> {
                     displayResultsModel.checkSimilarity.cognitiveMicrosoft[1].faceAttributes.emotion.contempt.ToString(),
                     displayResultsModel.checkSimilarity.cognitiveMicrosoft[1].faceAttributes.emotion.disgust.ToString(),
                     displayResultsModel.checkSimilarity.cognitiveMicrosoft[1].faceAttributes.emotion.anger.ToString(),
                     displayResultsModel.checkSimilarity.cognitiveMicrosoft[1].faceAttributes.emotion.fear.ToString(),
                     displayResultsModel.checkSimilarity.cognitiveMicrosoft[1].faceAttributes.emotion.happiness.ToString(),
                     displayResultsModel.checkSimilarity.cognitiveMicrosoft[1].faceAttributes.emotion.neutral.ToString(),
                     displayResultsModel.checkSimilarity.cognitiveMicrosoft[1].faceAttributes.emotion.sadness.ToString(),
                     displayResultsModel.checkSimilarity.cognitiveMicrosoft[1].faceAttributes.smile.ToString(),
                     displayResultsModel.checkSimilarity.cognitiveMicrosoft[1].faceAttributes.emotion.surprise.ToString(),
                     displayResultsModel.checkSimilarity.similarityTest[1].Confidence.ToString(),
                     displayResultsModel.checkSimilarity.cognitiveMicrosoft[1].faceAttributes.glasses.ToString()
                };
                List<string> childrenResults = new List<string> {
                     displayResultsModel.checkSimilarity.cognitiveMicrosoft[2].faceAttributes.emotion.contempt.ToString(),
                     displayResultsModel.checkSimilarity.cognitiveMicrosoft[2].faceAttributes.emotion.disgust.ToString(),
                     displayResultsModel.checkSimilarity.cognitiveMicrosoft[2].faceAttributes.emotion.anger.ToString(),
                     displayResultsModel.checkSimilarity.cognitiveMicrosoft[2].faceAttributes.emotion.fear.ToString(),
                     displayResultsModel.checkSimilarity.cognitiveMicrosoft[2].faceAttributes.emotion.happiness.ToString(),
                     displayResultsModel.checkSimilarity.cognitiveMicrosoft[2].faceAttributes.emotion.neutral.ToString(),
                     displayResultsModel.checkSimilarity.cognitiveMicrosoft[2].faceAttributes.emotion.sadness.ToString(),
                     displayResultsModel.checkSimilarity.cognitiveMicrosoft[2].faceAttributes.smile.ToString(),
                     displayResultsModel.checkSimilarity.cognitiveMicrosoft[2].faceAttributes.emotion.surprise.ToString(),
                     "-",
                     displayResultsModel.checkSimilarity.cognitiveMicrosoft[2].faceAttributes.glasses.ToString()
                };

                FragmentTransaction fragmentManager = this.FragmentManager.BeginTransaction();
                DisplayResults3PersonsSimilarity diplaResults = new DisplayResults3PersonsSimilarity(attributes,fatherResults, motherResults, childrenResults);
                fragmentManager.Replace(Resource.Id.parent_fragment, diplaResults);
                fragmentManager.SetTransition(FragmentTransit.FragmentOpen);
                fragmentManager.AddToBackStack(null);
                fragmentManager.Commit();
                progressDialog.Hide();
            }
            catch(HttpRequestException httpExec)
            {
                progressDialog.Hide();
                displaySnackBar("Check your internet connection.");
            }
            catch (Exception ex)
            {
                progressDialog.Hide();
                displaySnackBar(ex.ToString());
            }

        }

        private void HandleChildrenChoose(object sender, EventArgs e)
        {
            Intent intent = new Intent();
            intent.SetType("image/*");
            intent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(intent, "Choose children image"), imageChildrenId);
        }

        private void HandleMotherChoose(object sender, EventArgs e)
        {
            Intent intent = new Intent();
            intent.SetType("image/*");
            intent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(intent, "Choose mother image"), imageMotherId);
        }

        private void HandleFatherChoose(object sender, EventArgs e)
        {
            Intent intent = new Intent();
            intent.SetType("image/*");
            intent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(intent, "Choose father image"), imageFatherId);
        }

        public override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if ((requestCode == imageFatherId) && (resultCode == Result.Ok) && (data != null))
            {
                imgvFather.SetImageURI(data.Data);
            }
            else if ((requestCode == imageMotherId) && (resultCode == Result.Ok) && (data != null))
            {
                imgvMother.SetImageURI(data.Data);
            }
            else if ((requestCode == imageChildrenId) && (resultCode == Result.Ok) && (data != null))
            {
                imgvChildren.SetImageURI(data.Data);
            }
        }

        private async Task<string> convertImageToBase64(ImageView image)
        {
            BitmapDrawable bd = (BitmapDrawable)image.Drawable;
            Bitmap bitmap = bd.Bitmap;
            byte[] imageData;
            MemoryStream stream = new MemoryStream();
            await bitmap.CompressAsync(Bitmap.CompressFormat.Jpeg, 90, stream);
            imageData = stream.ToArray();
            return Convert.ToBase64String(imageData);
        }

        private void displaySnackBar(string text)
        {
            Snackbar bar = Snackbar.Make(view, Html.FromHtml("<font color=\"#000000\">" + text + "</font>"), Snackbar.LengthLong);
            Snackbar.SnackbarLayout layout = (Snackbar.SnackbarLayout)bar.View;
            layout.SetMinimumHeight(100);
            layout.SetBackgroundColor(Android.Graphics.Color.White);
            layout.TextAlignment = TextAlignment.Center;
            layout.ScrollBarSize = 20;
            bar.SetActionTextColor(Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Red));
            bar.SetDuration(Snackbar.LengthIndefinite);
            bar.SetAction("Ok", (v) => { });
            bar.Show();
        }
    }
}