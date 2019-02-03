using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Text;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using AndroidRela.Models.SignUp;
using static Android.App.DatePickerDialog;

namespace AndroidRela.Controllers
{
    [Activity(Label = "Register", Theme = "@style/AppTheme")]
    public class Register : Activity, IOnDateSetListener
    {
        private const int DATE_DIALOG = 1;
        private int year, month, day;
        private HttpClient httpClient;

        private EditText Email, Username, Password, ConfirmPassword, FirstName, LastName, PhoneNumber, txtBirthday;
        private Button btnBirthday;
        private RadioButton Male, Female, NotSpecified;
        private ImageView UserImage;
        private Button ChooseImage, btnRegister;
        private ImageView profileImage;
        public static readonly int imageId = 1000;
        private LinearLayout linearLayout2;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Register);

            Email = FindViewById<EditText>(Resource.Id.Email);
            Username = FindViewById<EditText>(Resource.Id.Username);
            Password = FindViewById<EditText>(Resource.Id.Password);
            ConfirmPassword = FindViewById<EditText>(Resource.Id.ConfirmPassword);
            FirstName = FindViewById<EditText>(Resource.Id.FirstName);
            LastName = FindViewById<EditText>(Resource.Id.LastName);
            PhoneNumber = FindViewById<EditText>(Resource.Id.PhoneNumber);
            Male = FindViewById<RadioButton>(Resource.Id.Male);
            Female = FindViewById<RadioButton>(Resource.Id.Female);
            NotSpecified = FindViewById<RadioButton>(Resource.Id.NotSpecified);
            ChooseImage = FindViewById<Button>(Resource.Id.ChooseImage);
            btnRegister = FindViewById<Button>(Resource.Id.btnRegister);
            UserImage = FindViewById<ImageView>(Resource.Id.UserImage);
            btnBirthday = FindViewById<Button>(Resource.Id.btnBirthday);
            txtBirthday = FindViewById<EditText>(Resource.Id.txtBirthday);
            linearLayout2 = FindViewById<LinearLayout>(Resource.Id.linearLayout2);

            btnBirthday.Click += handleBirthdayButton;
            ChooseImage.Click += clickChooseImage;
            btnRegister.Click += clickBtnRegister;
        }

        private void handleBirthdayButton(object sender, EventArgs e)
        {
            ShowDialog(DATE_DIALOG);

        }

        protected override Dialog OnCreateDialog(int id)
        {
            switch (id) {
                case DATE_DIALOG:
                        return new DatePickerDialog(this, this, year, month, day);
                    break;
            }
            return null;
        }


        private async void clickBtnRegister(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Email.Text) || string.IsNullOrWhiteSpace(Email.Text))
                displaySnackBar("Fill Email!");
            else if (string.IsNullOrEmpty(Username.Text) || string.IsNullOrWhiteSpace(Username.Text))
                displaySnackBar("Fill username!");
            else if (string.IsNullOrEmpty(Password.Text) || string.IsNullOrWhiteSpace(Password.Text))
                displaySnackBar("Fill password!");
            else if (string.IsNullOrEmpty(ConfirmPassword.Text) || string.IsNullOrWhiteSpace(ConfirmPassword.Text))
                displaySnackBar("Fill column confirm password!");
            else if (string.IsNullOrEmpty(FirstName.Text) || string.IsNullOrWhiteSpace(FirstName.Text))
                displaySnackBar("Fill First name!");
            else if (string.IsNullOrEmpty(LastName.Text) || string.IsNullOrWhiteSpace(LastName.Text))
                displaySnackBar("Fill Last name!");
            else if (Password.Text != ConfirmPassword.Text)
                displaySnackBar("Confirm password do not match with password. Please check them.");
            else
            {
                var progressDialog = new ProgressDialog(this);
                progressDialog.SetCancelable(true);
                progressDialog.SetMessage("Please wait!");
                progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
                progressDialog.Show();

                string email = Email.Text;
                string username = Username.Text;
                string password = Password.Text;
                string confirmPassword = ConfirmPassword.Text;
                string fistName = FirstName.Text;
                string lastName = LastName.Text;
                string phoneNumber = PhoneNumber.Text;

                httpClient = new HttpClient();

                var resultForUsername = await httpClient.PostAsync("UrlApiapi/account/checkUsername", new StringContent(JsonConvert.SerializeObject(username), Encoding.UTF8, "application/json"));
                var responseForUsername = await resultForUsername.Content.ReadAsStringAsync();

                var resultForEmail = await httpClient.PostAsync("UrlApiapi/account/checkEmail", new StringContent(JsonConvert.SerializeObject(email), Encoding.UTF8, "application/json"));
                var responseForEmail = await resultForEmail.Content.ReadAsStringAsync();

                if (responseForUsername == "true")
                {
                    displaySnackBar("There exist user with this username.");
                    progressDialog.Hide();
                }
                else if (responseForEmail == "true")
                {
                    displaySnackBar("There exisit new user with this mail.");
                    progressDialog.Hide();
                }
                else
                {
                    string base64Image = "";
                    try
                    {
                        UserImage.BuildDrawingCache(true);
                        BitmapDrawable bd = (BitmapDrawable)UserImage.Drawable;
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

                    try
                    {
                        var signUp = new SignUp
                        {
                            Email = email,
                            UserName = username,
                            FirstName = fistName,
                            LastName = lastName,
                            BirthDate = DateTime.Now.ToString(),
                            Password = password,
                            PhoneNumber = phoneNumber,
                            user_image = base64Image,
                            ConfirmPassword = confirmPassword,
                            Gender = "M"
                        };

                        var httpClient = new HttpClient();
                        var jsonContent = JsonConvert.SerializeObject(signUp);
                        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                        var response = await httpClient.PostAsync("UrlApiapi/account/Register", content);
                        var responseString = await response.Content.ReadAsStringAsync();
                        Toast.MakeText(this, "Check your email for further details.", ToastLength.Long).Show();
                        progressDialog.Cancel();
                        StartActivity(typeof(LogIn));
                    }
                    catch (HttpRequestException httpEx)
                    {
                        Snackbar bar = Snackbar.Make(linearLayout2, "Please check your internet connection!", Snackbar.LengthLong);
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
                        progressDialog.Cancel();
                        Toast.MakeText(this, "Error: " + ex, ToastLength.Long).Show();
                    }
                }
            }
        }

        private void clickChooseImage(object sender, EventArgs e)
        {
            Intent = new Intent();
            Intent.SetType("image/*");
            Intent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(Intent, "Select Picture"), imageId);
        }
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if ((requestCode == imageId) && (resultCode == Result.Ok) && (data != null))
            {
                UserImage.SetImageURI(data.Data);
            }

            base.OnActivityResult(requestCode, resultCode, data);
        }

        public void OnDateSet(DatePicker view, int year, int month, int dayOfMonth)
        {
            this.year = year;
            this.month = month;
            this.day = dayOfMonth;
            txtBirthday.Text = dayOfMonth + "-" + month + "-" + year;
        }

        private void displaySnackBar(string text)
        {
            Snackbar bar = Snackbar.Make(linearLayout2, Html.FromHtml("<font color=\"#000000\">"+text+"</font>"), Snackbar.LengthLong);
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