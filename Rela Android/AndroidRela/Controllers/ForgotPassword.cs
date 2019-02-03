using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Text;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;

namespace AndroidRela.Controllers
{
    [Activity(Label = "ForgotPassword", Theme = "@style/AppTheme")]
    public class ForgotPassword : Activity
    {
        private EditText email;
        private Button btnRecover;
        private LinearLayout parentForgot;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ForgotPassword);
            email = FindViewById<EditText>(Resource.Id.txtEmail);
            btnRecover = FindViewById<Button>(Resource.Id.btnRecover);
            parentForgot = FindViewById<LinearLayout>(Resource.Id.parentForgot);

            btnRecover.Click += bntRecoverClick;
        }

        private async void bntRecoverClick(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email.Text) || string.IsNullOrEmpty(email.Text))
                {
                    Snackbar bar = Snackbar.Make(parentForgot, Html.FromHtml("<font color=\"#000000\">Fill email</font>"), Snackbar.LengthLong);
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

                    string txtEmailForgot = email.Text;
                    var httpClient = new HttpClient();
                    var content = new StringContent(JsonConvert.SerializeObject(new { Email = txtEmailForgot }), Encoding.UTF8, "application/json");
                    var result = await httpClient.PostAsync("UrlApi" + "api/account/forgotpassword", content);
                    var response = await result.Content.ReadAsStringAsync();
                    Toast.MakeText(this, "Check your email please.", ToastLength.Long).Show();
                    StartActivity(typeof(LogIn));
                }
            }
            catch (HttpRequestException httpEx)
            {
                Snackbar bar = Snackbar.Make(parentForgot, Html.FromHtml("<font color=\"#000000\">Check your internet connection</font>"), Snackbar.LengthLong);
                Snackbar.SnackbarLayout layout = (Snackbar.SnackbarLayout)bar.View;
                layout.SetMinimumHeight(100);
                layout.SetBackgroundColor(Android.Graphics.Color.White);
                layout.TextAlignment = TextAlignment.Center;
                layout.ScrollBarSize = 16;
                bar.SetActionTextColor(Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Red));
                bar.SetDuration(Snackbar.LengthIndefinite);
                bar.SetAction("Ok", (v) => { });
                bar.Show();
            }
            catch (Exception ex)
            {
                Snackbar bar = Snackbar.Make(parentForgot, Html.FromHtml("<font color=\"#000000\">Error: "+ex+"</font>"), Snackbar.LengthLong);
                Snackbar.SnackbarLayout layout = (Snackbar.SnackbarLayout)bar.View;
                layout.SetMinimumHeight(100);
                layout.SetBackgroundColor(Android.Graphics.Color.White);
                layout.TextAlignment = TextAlignment.Center;
                layout.ScrollBarSize = 16;
                bar.SetActionTextColor(Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Red));
                bar.SetDuration(Snackbar.LengthIndefinite);
                bar.SetAction("Ok", (v) => { });
                bar.Show();
            }
        }
    }
}