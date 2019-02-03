using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using AndroidRela.Controllers;
using AndroidRela.Fragments;
using AndroidRela.Models.CheckSimilarity;
using AndroidRela.Models.SqlLite;
using AndroidRela.Models.Voice;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AndroidRela
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
    public class MainActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        TextView textMessage;
        private SQLiteConnection connection;
        private string dbPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "users.db3");
        private ImageView profileImage;
        private TextView txtHello;
        private ImageButton btnLogOf;
        private ScrollView scrollView1;
        LinearLayout mainLayout;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            FragmentTransaction fragmentTx = this.FragmentManager.BeginTransaction();
            Main mainFragment = new Main();

            fragmentTx.Add(Resource.Id.parent_fragment, mainFragment);
            fragmentTx.SetTransition(FragmentTransit.FragmentOpen);
            fragmentTx.AddToBackStack(null);
            fragmentTx.Commit();

            mainLayout = FindViewById<LinearLayout>(Resource.Id.linearLayout1);
            textMessage = FindViewById<TextView>(Resource.Id.message);

            var navigation = FindViewById<BottomNavigationView>(Resource.Id.navigationBar);
            navigation.SetOnNavigationItemSelectedListener(this);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.headerToolbar);
            SetSupportActionBar(toolbar);

            profileImage = FindViewById<ImageView>(Resource.Id.UserProfileImage);
            btnLogOf = FindViewById<ImageButton>(Resource.Id.btnLogOf);

            btnLogOf.Click += handleBtnLogOf;

            txtHello = FindViewById<TextView>(Resource.Id.txtHelloPerson);
            connection = new SQLiteConnection(dbPath);
            var userData = connection.Get<UserSession>(1);
            txtHello.Text = "";
            txtHello.Text = userData.FirstName + " " + userData.LastName;
            try
            {
                var byteImage = Convert.FromBase64String(userData.user_image);
                Bitmap image = await BitmapFactory.DecodeByteArrayAsync(byteImage, 0, byteImage.Length);
                profileImage.SetImageBitmap(image);
            }
            catch (Exception ex)
            {
            }
        }

        private void handleBtnLogOf(object sender, EventArgs e)
        {
            connection.DropTable<UserSession>();
            StartActivity(typeof(LogIn));
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.navigation_home:
                    try
                    {
                        FragmentTransaction fragmentTx = this.FragmentManager.BeginTransaction();
                        Main mainFragment = new Main();
                        fragmentTx.Replace(Resource.Id.parent_fragment, mainFragment);
                        fragmentTx.SetTransition(FragmentTransit.FragmentOpen);
                        fragmentTx.AddToBackStack(null);
                        fragmentTx.Commit();
                        return true;
                    }catch(Exception ex){
                        return false;
                    }
                case Resource.Id.ImagesProcesed:
                    callFragmentImagesHistory();
                    return true;

                case Resource.Id.voiceProcesed:
                    callFragmentImagesProcesed();
                    return true;
            }
            return false;
        }

        public async Task callFragmentImagesHistory()
        {
            var progressDialog = new ProgressDialog(this);
            try
            {
                progressDialog.SetIcon(2130968582);
                progressDialog.SetCancelable(true);
                progressDialog.SetMessage("Please wait!");
                progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
                progressDialog.Show();
                connection = new SQLiteConnection(dbPath);
                var userData = connection.Get<UserSession>(1);
                var httpClient = new HttpClient();
                var content = new StringContent(JsonConvert.SerializeObject(new { userId = userData.userId }), Encoding.UTF8, "application/json");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userData.Token);
                var response = await httpClient.PostAsync("UrlApi" + "api/History/GetImages", content);
                var result = await response.Content.ReadAsStringAsync();
                var listOfImages = JsonConvert.DeserializeObject<List<HistoryOfProcesedImages>>(result);

                FragmentTransaction fragmentTxHistory = this.FragmentManager.BeginTransaction();
                ImagesProcesedHistory imagesFragment = new ImagesProcesedHistory(listOfImages);
                fragmentTxHistory.Replace(Resource.Id.parent_fragment, imagesFragment);
                fragmentTxHistory.SetTransition(FragmentTransit.EnterMask);
                fragmentTxHistory.AddToBackStack(null);
                fragmentTxHistory.Commit();
                progressDialog.Hide();
            }catch(Exception ex)
            {
                progressDialog.Hide();
            }
        }

        public async Task callFragmentImagesProcesed() {
            var progressDialog = new ProgressDialog(this);
            try
            {
                progressDialog.SetIcon(2130968582);
                progressDialog.SetCancelable(true);
                progressDialog.SetMessage("Please wait!");
                progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
                progressDialog.Show();
                connection = new SQLiteConnection(dbPath);
                var userData = connection.Get<UserSession>(1);
                var httpClient = new HttpClient();
                var content = new StringContent(JsonConvert.SerializeObject(new { userId = userData.userId }), Encoding.UTF8, "application/json");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userData.Token);
                var response = await httpClient.PostAsync("UrlApi" + "api/History/GetVoiceImage", content);
                var result = await response.Content.ReadAsStringAsync();
                var listOfImages = JsonConvert.DeserializeObject<List<VoiceHistory>>(result);

                FragmentTransaction fragmentTransaction = this.FragmentManager.BeginTransaction();
                ImageVoiceHistory imageVoiceHistoryFragment = new ImageVoiceHistory(listOfImages);
                fragmentTransaction.Replace(Resource.Id.parent_fragment, imageVoiceHistoryFragment);
                fragmentTransaction.SetTransition(FragmentTransit.FragmentFade);
                fragmentTransaction.AddToBackStack(null);
                fragmentTransaction.Commit();
                progressDialog.Hide();
            }
            catch(Exception ex)
            {
                progressDialog.Hide();
            }
        }

    }
}

