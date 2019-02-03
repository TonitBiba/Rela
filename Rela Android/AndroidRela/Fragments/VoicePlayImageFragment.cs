using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace AndroidRela.Fragments
{
    public class VoicePlayImageFragment : Fragment
    {
        private Android.Support.Design.Widget.FloatingActionButton btnDownload, btnPlay;
        private MediaPlayer mediaPlayer;
        public VoicePlayImageFragment(MediaPlayer mediaPlayer)
        {
            this.mediaPlayer = mediaPlayer;
        }
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.VoicePlayImage, container, false);
            btnDownload = view.FindViewById<Android.Support.Design.Widget.FloatingActionButton>(Resource.Id.btnDownload);
            btnPlay = view.FindViewById<Android.Support.Design.Widget.FloatingActionButton>(Resource.Id.btnPlay);
            btnPlay.Click += HandleBtnPlay;
            mediaPlayer.Completion += handleCompletitionEvent;
            return view;
        }

        private void handleCompletitionEvent(object sender, EventArgs e)
        {
            btnPlay.SetImageResource(Resource.Mipmap.play);
        }

        private void HandleBtnPlay(object sender, EventArgs e)
        {
            if (mediaPlayer.IsPlaying)
            {
                btnPlay.SetImageResource(Resource.Mipmap.play);
                mediaPlayer.Pause();
            }
            else
            {
                btnPlay.SetImageResource(Resource.Mipmap.pause);
                mediaPlayer.Start();
            }
        }
    }
}