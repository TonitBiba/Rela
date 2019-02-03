using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Text;
using Android.Views;
using Android.Widget;
using AndroidRela.Models.LogIn;
using AndroidRela.Models.SqlLite;
using Newtonsoft.Json;
using SQLite;

namespace AndroidRela.Controllers
{
    [Activity(Label = "Rela project", MainLauncher = true, Theme = "@style/LogIntheme")]
    public class LogIn : Activity
    {
        private EditText username, password;
        private Button btnLogIn, btnForgotPassword, btnSignUp;
        private SQLiteConnection con;
        private RelativeLayout parentLayout;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                SetContentView(Resource.Layout.LogIn);
            }
            catch (Exception ex)
            {
            }
            var logIn = new LogIn();
            username = FindViewById<EditText>(Resource.Id.edtUsername);
            password = FindViewById<EditText>(Resource.Id.edtPassword);
            btnLogIn = FindViewById<Button>(Resource.Id.btnLogIn);
            btnForgotPassword = FindViewById<Button>(Resource.Id.btnForgotPassword);
            btnSignUp = FindViewById<Button>(Resource.Id.btnSignUp);
            parentLayout = FindViewById<RelativeLayout>(Resource.Id.parentLayout);
            btnLogIn.Click += clickBtnLogIn;
            btnForgotPassword.Click += clickBtnForgotPassword;
            btnSignUp.Click += clickBtnSignUp;

            try
            {
                string dpPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "users.db3");
                con = new SQLiteConnection(dpPath);
                con.DropTable<UserSession>();
                con.CreateTable<UserSession>();
            }
            catch (Exception ex)
            {
            }
        }

        private void clickBtnSignUp(object sender, EventArgs e)
        {
            StartActivity(typeof(Register));
        }

        private void clickBtnForgotPassword(object sender, EventArgs e)
        {
            StartActivity(typeof(ForgotPassword));
        }

        private async void clickBtnLogIn(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(username.Text) || string.IsNullOrEmpty(username.Text))
            {
                Snackbar bar = Snackbar.Make(parentLayout, "Fill username", Snackbar.LengthLong);
                bar.SetText(Html.FromHtml("<font color=\"#000000\">Fill username</font>"));
                Snackbar.SnackbarLayout layout = (Snackbar.SnackbarLayout)bar.View;
                layout.SetMinimumHeight(100);
                layout.SetBackgroundColor(Android.Graphics.Color.White);
                layout.TextAlignment = TextAlignment.Center;
                layout.ScrollBarSize = 16;
                bar.SetActionTextColor(Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Red));
                bar.SetDuration(Snackbar.LengthLong);
                bar.SetAction("Ok", (v) => { });
                bar.Show();
            }
            else if (string.IsNullOrWhiteSpace(password.Text) || string.IsNullOrEmpty(password.Text))
            {
                Snackbar bar = Snackbar.Make(parentLayout, Html.FromHtml("<font color=\"#000000\">Fill password</font>"), Snackbar.LengthLong);
                Snackbar.SnackbarLayout layout = (Snackbar.SnackbarLayout)bar.View;
                layout.SetMinimumHeight(100);
                layout.SetBackgroundColor(Android.Graphics.Color.White);
                layout.TextAlignment = TextAlignment.Center;
                layout.ScrollBarSize = 16;
                bar.SetActionTextColor(Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Red));
                bar.SetDuration(Snackbar.LengthLong);
                bar.SetAction("Ok", (v) => { });
                bar.Show();
            }
            else
            {
                var progressDialog = new ProgressDialog(this);
                try
                {
                    progressDialog.SetIcon(2130968582);
                    progressDialog.SetCancelable(true);
                    progressDialog.SetMessage("Please wait!");
                    progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
                    progressDialog.Show();

                    var client = new HttpClient();
                    var keyValueLogIn = new List<KeyValuePair<string, string>>
                  {
                      new KeyValuePair<string, string>("username", username.Text),
                      new KeyValuePair<string, string>("password",password.Text),
                      new KeyValuePair<string, string>("grant_type","password")
                  };
                    var request = new HttpRequestMessage(HttpMethod.Post, "UrlApiToken");
                    request.Content = new FormUrlEncodedContent(keyValueLogIn);
                    var response = await client.SendAsync(request);
                    var content = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        TokenModel tokenFromServer = JsonConvert.DeserializeObject<TokenModel>(content);
                        var jsonContent = JsonConvert.SerializeObject(username.Text);
                        var LogIncontent = new StringContent(jsonContent, Encoding.ASCII, "application/json");

                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenFromServer.access_token);

                        response = await client.PostAsync("UrlApi" + "api/Account/userdata", LogIncontent);
                        var userData = await response.Content.ReadAsStringAsync();

                        User user = JsonConvert.DeserializeObject<User>(userData);

                        var userSession = new UserSession
                        {
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            userId = user.userId,
                            Token = tokenFromServer.access_token,
                            user_image = user.user_image
                        };
                        con.Insert(userSession);
                        progressDialog.Cancel();
                        StartActivity(typeof(MainActivity));
                    }
                    else
                    {
                        LogInError logInError = JsonConvert.DeserializeObject<LogInError>(content);
                        var alertLogInError = new Android.App.AlertDialog.Builder(this);
                        alertLogInError.SetTitle(logInError.error);
                        alertLogInError.SetMessage(logInError.error_description);
                        username.Text = "";
                        password.Text = "";

                        Snackbar bar = Snackbar.Make(parentLayout, Html.FromHtml("<font color=\"#000000\">" + logInError.error + " - " + logInError.error_description + "</font>"), Snackbar.LengthLong);
                        Snackbar.SnackbarLayout layout = (Snackbar.SnackbarLayout)bar.View;
                        layout.SetMinimumHeight(100);
                        layout.SetBackgroundColor(Android.Graphics.Color.White);
                        layout.TextAlignment = TextAlignment.Center;
                        layout.ScrollBarSize = 16;
                        bar.SetActionTextColor(Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Red));
                        bar.SetDuration(Snackbar.LengthLong);
                        bar.SetAction("Ok", (v) => { });
                        bar.Show();

                        progressDialog.Cancel();
                    }
                }
                catch (HttpRequestException httpEx)
                {
                    Snackbar bar = Snackbar.Make(parentLayout, Html.FromHtml("<font color=\"#000000\">Please check your internet connection!</font>"), Snackbar.LengthLong);
                    Snackbar.SnackbarLayout layout = (Snackbar.SnackbarLayout)bar.View;
                    layout.SetMinimumHeight(100);
                    layout.SetBackgroundColor(Android.Graphics.Color.White);
                    layout.TextAlignment = TextAlignment.Center;
                    layout.ScrollBarSize = 16;
                    bar.SetActionTextColor(Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Red));
                    bar.SetDuration(Snackbar.LengthIndefinite);
                    bar.SetAction("Ok", (v) => { });
                    bar.Show();
                    progressDialog.Cancel();
                }
                catch (Exception ex)
                {
                    Snackbar bar = Snackbar.Make(parentLayout, Html.FromHtml("<font color=\"#000000\">Error: " + ex + "</font>"), Snackbar.LengthLong);
                    Snackbar.SnackbarLayout layout = (Snackbar.SnackbarLayout)bar.View;
                    layout.SetMinimumHeight(100);
                    layout.SetBackgroundColor(Android.Graphics.Color.White);
                    layout.TextAlignment = TextAlignment.Center;
                    layout.ScrollBarSize = 16;
                    bar.SetActionTextColor(Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Red));
                    bar.SetDuration(Snackbar.LengthIndefinite);
                    bar.SetAction("Ok", (v) => { });
                    bar.Show();
                    progressDialog.Cancel();
                }
            }
        }
    }
}