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
    public class CheckSimilarityFragment : Fragment
    {
        private TextView txtToolbarText;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            FragmentTransaction fragmentManager = this.FragmentManager.BeginTransaction();
            FamilyThreePhotoFragment familyFragment = new FamilyThreePhotoFragment();
            fragmentManager.Replace(Resource.Id.CheckSimilarityFrame, familyFragment);
            fragmentManager.SetTransition(FragmentTransit.FragmentOpen);
            fragmentManager.AddToBackStack(null);
            fragmentManager.Commit();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        { 
            View view = inflater.Inflate(Resource.Layout.CheckSimilarityFragment, container, false);
            return view;
        }
    }
}