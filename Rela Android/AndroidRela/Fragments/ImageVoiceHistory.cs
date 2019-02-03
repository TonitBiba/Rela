using System;
using System.Collections.Generic;
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
using Android.Util;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using AndroidRela.Adapters;
using AndroidRela.Models.SqlLite;
using AndroidRela.Models.Voice;
using SQLite;
using AndroidRela.Models.CheckSimilarity;

namespace AndroidRela.Fragments
{
    public class ImageVoiceHistory : Fragment
    {
        View view;
        private SQLiteConnection connection;
        private string dbPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "users.db3");
        private ListView voiceProcesedMenu;
        private List<VoiceHistory> listOfImages;

        public ImageVoiceHistory(List<VoiceHistory> listOfImages)
        {
            this.listOfImages = listOfImages;
        }



        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.ImageVoiceHistory, container, false);
            voiceProcesedMenu = view.FindViewById<ListView>(Resource.Id.voiceProcesedMenu);
            try
            {
                if (listOfImages.Count > 0)
                {
                    voiceProcesedMenu.Adapter = new ImagesVoiceProcesedAdapter(view.Context, listOfImages);
                }
                else
                    displaySnackBar("You do not have any image procesed.");
            }
            catch (HttpRequestException httpEx)
            {
                displaySnackBar("Check your internet connection!");
            }
            catch (Exception ex)
            {
                Snackbar.Make(view, "Error:" + ex, Snackbar.LengthShort).Show();
            }
            return view;
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