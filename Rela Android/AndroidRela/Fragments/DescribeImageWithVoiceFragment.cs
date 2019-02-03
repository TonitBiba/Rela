using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using AndroidRela.Models.SqlLite;
using AndroidRela.Models.Voice;
using SQLite;

namespace AndroidRela.Fragments
{
    public class DescribeImageWithVoiceFragment : Fragment
    {
        MediaPlayer mediaPlayer;
        private ImageView imgVoiceDescription;
        private Button btnAnalyzeVoiceImage;
        public static readonly int imageId = 1000;
        private View view;
        private SQLiteConnection connection;
        private Android.Support.Design.Widget.FloatingActionButton imgButnChooseVoiceImage;
        private string dbPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "users.db3");

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.DescribeImageWithVoiceFragment, container, false);
            imgVoiceDescription = view.FindViewById<ImageView>(Resource.Id.voiceImage);
            btnAnalyzeVoiceImage = view.FindViewById<Button>(Resource.Id.btnAnalyzeVoiceImage);
            imgButnChooseVoiceImage = view.FindViewById<Android.Support.Design.Widget.FloatingActionButton>(Resource.Id.imgButnChooseVoiceImage);
            imgButnChooseVoiceImage.Click += handleChooseVoiceImage;
            btnAnalyzeVoiceImage.Click += handleAnalyzeVoiceImage;
            return view;
        }

        private async void handleAnalyzeVoiceImage(object sender, EventArgs e)
        {
            var progressDialog = new ProgressDialog(view.Context);
                string base64Image = "";
                try
                {
                progressDialog.SetIcon(2130968582);
                progressDialog.SetCancelable(true);
                progressDialog.SetMessage("Please wait!");
                progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
                progressDialog.Show();
                BitmapDrawable bd = (BitmapDrawable)imgVoiceDescription.Drawable;
                    Bitmap bitmap = bd.Bitmap;
                    byte[] imageData;
                    using (MemoryStream stream = new MemoryStream())
                    {
                        bitmap.Compress(Bitmap.CompressFormat.Png, 0, stream);
                        imageData = stream.ToArray();
                    }
                    base64Image = Convert.ToBase64String(imageData);
                }
                catch (Exception ex) { }

            try { 
                var httpClient = new HttpClient();

                connection = new SQLiteConnection(dbPath);
                var userData = connection.Get<UserSession>(1);

                var jsonContent = JsonConvert.SerializeObject(new { userId = userData.userId, base64Image = base64Image });
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userData.Token);
                var response = await httpClient.PostAsync("UrlApi" + "api/FaceRecognition/DescribeImageWithVoice", content);
                var responseString = await response.Content.ReadAsStringAsync();
                var describeImageObject = JsonConvert.DeserializeObject<DescribeImageVoice>(responseString);
                describeImageObject.imageBase64 = base64Image;

                var destination = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "rela1.mp3");
                File.WriteAllBytes(destination, Convert.FromBase64String(describeImageObject.voiceBase64));

                mediaPlayer = new MediaPlayer();
                mediaPlayer.SetDataSource(destination);
                mediaPlayer.SetAudioStreamType(Android.Media.Stream.Music);
                mediaPlayer.Prepare();
                VoicePlayImageFragment voiceFragment = new VoicePlayImageFragment(mediaPlayer);
                FragmentTransaction fragmentManager = this.FragmentManager.BeginTransaction();
                fragmentManager.Replace(Resource.Id.parent_voice_fragment, voiceFragment);
                fragmentManager.SetTransition(FragmentTransit.FragmentOpen);
                fragmentManager.AddToBackStack(null);
                fragmentManager.Commit();
                progressDialog.Hide();
            }
            catch(HttpRequestException httpExeption)
            {
                progressDialog.Hide();
                displaySnackBar("Check your internet connection!");
            }
            catch (Exception ex) {
                progressDialog.Hide();
                displaySnackBar(ex.ToString());
            }
        }

        private void handleChooseVoiceImage(object sender, EventArgs e)
        {
            Intent intent = new Intent();
            intent.SetType("image/*");
            intent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(intent, "Select Picture"), imageId);
        }

        public override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if ((requestCode == imageId) && (resultCode == Result.Ok) && (data != null))
            {
                imgVoiceDescription.SetImageURI(data.Data);
            }
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