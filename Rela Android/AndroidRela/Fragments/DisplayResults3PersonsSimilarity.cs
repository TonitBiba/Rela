using System;
using System.Collections.Generic;
using System.Linq;
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
using AndroidRela.Models.CheckSimilarity;

namespace AndroidRela.Fragments
{
    public class DisplayResults3PersonsSimilarity : Fragment
    {
        List<string> father, mother, children, listAttributes;
        private ListView listFather, listChildren, listMother, listElements;
        private View view;
        public DisplayResults3PersonsSimilarity(List<string> listAttributes, List<string> father, List<string> mother, List<string> children)
        {
            this.listAttributes = listAttributes;
            this.father = father;
            this.mother = mother;
            this.children = children;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.DisplayResults3PersonSimilarity, container, false);
            listFather = view.FindViewById<ListView>(Resource.Id.listFather);
            listChildren = view.FindViewById<ListView>(Resource.Id.listChildren);
            listMother = view.FindViewById<ListView>(Resource.Id.listMother);
            listElements = view.FindViewById<ListView>(Resource.Id.listElements);


            ArrayAdapter<string> adapterAttrbutes = new ArrayAdapter<string>(view.Context, Android.Resource.Layout.SimpleListItem1, listAttributes);
            listElements.Adapter = adapterAttrbutes;

            ArrayAdapter<string> adapterFather = new ArrayAdapter<string>(view.Context, Android.Resource.Layout.SimpleListItem1, father);
            listFather.Adapter = adapterFather;

            ArrayAdapter<string> adapterMother = new ArrayAdapter<string>(view.Context, Android.Resource.Layout.SimpleListItem1, mother);
            listMother.Adapter = adapterMother;

            ArrayAdapter<string> adapterChildren = new ArrayAdapter<string>(view.Context, Android.Resource.Layout.SimpleListItem1, children);
            listChildren.Adapter = adapterChildren;

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