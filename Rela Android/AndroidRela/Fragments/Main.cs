using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace AndroidRela.Fragments
{
    public class Main : Fragment
    {
        private ImageView btnCheckSimilarity, btnImageToVoice;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        private void HandleImageToVoice(object sender, EventArgs e)
        {
            FragmentTransaction fragmentManager = this.FragmentManager.BeginTransaction();
            DescribeImageWithVoiceFragment voice = new DescribeImageWithVoiceFragment();
            fragmentManager.Replace(Resource.Id.parent_fragment, voice);
            fragmentManager.SetTransition(FragmentTransit.FragmentOpen);
            fragmentManager.AddToBackStack(null);
            fragmentManager.Commit();
        }

        private void HandleCheckSimilarity(object sender, EventArgs e)
        {
            FragmentTransaction fragmentManager = this.FragmentManager.BeginTransaction();
            CheckSimilarityFragment checkSimilarityFragment = new CheckSimilarityFragment();
            fragmentManager.Replace(Resource.Id.parent_fragment, checkSimilarityFragment);
            fragmentManager.SetTransition(FragmentTransit.FragmentOpen);
            fragmentManager.AddToBackStack(null);
            fragmentManager.Commit();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.OptionMeny, container, false);
            btnCheckSimilarity = view.FindViewById<ImageView>(Resource.Id.btnCheckSimilarity);
            btnImageToVoice = view.FindViewById<ImageView>(Resource.Id.btnVoiceDescribe);
            btnCheckSimilarity.Click += HandleCheckSimilarity;
            btnImageToVoice.Click += HandleImageToVoice;
            return view;
        }
    }
}