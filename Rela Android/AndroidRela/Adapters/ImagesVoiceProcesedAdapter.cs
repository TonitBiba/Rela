using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidRela.Models.Voice;

namespace AndroidRela.Adapters
{
    class ImagesVoiceProcesedAdapter : BaseAdapter
    {
        List<VoiceHistory> voiceHistory;
        private LayoutInflater inflater;
        private MediaPlayer mediaPlayer;
        Context context;
        public ImagesVoiceProcesedAdapter(Context context, List<VoiceHistory> voiceHistory)
        {
            this.context = context;
            this.voiceHistory = voiceHistory;
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return voiceHistory.IndexOf(voiceHistory[position]);
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {

            if (inflater == null)
            {
                inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
            }

            if (convertView == null)
            {
                convertView = inflater.Inflate(Resource.Layout.menuVoiceHistoryImage, parent, false);
            }
            ImagesVoiceProcesedAdapterViewHolder holder = new ImagesVoiceProcesedAdapterViewHolder(convertView);
            holder.imageView1.SetImageBitmap(convertImage(voiceHistory[position].image));

            var destination = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "relaHistoryVoice" + position + ".mp3");
            File.WriteAllBytes(destination, Convert.FromBase64String(voiceHistory[position].GoogleVoice));

            mediaPlayer = new MediaPlayer();
            mediaPlayer.SetDataSource(destination);
            mediaPlayer.SetAudioStreamType(Android.Media.Stream.Music);
            mediaPlayer.Prepare();

            holder.floatingActionButton1.Click += delegate
            {
                if (mediaPlayer.IsPlaying)
                {
                    holder.floatingActionButton1.SetImageResource(Resource.Mipmap.play);
                    mediaPlayer.Pause();
                }
                else
                {
                    holder.floatingActionButton1.SetImageResource(Resource.Mipmap.pause);
                    mediaPlayer.Start();
                }
            };

            return convertView;
        }


        private Bitmap convertImage(string base64Image)
        {
            var byteImage = Convert.FromBase64String(base64Image);
            Bitmap image = BitmapFactory.DecodeByteArrayAsync(byteImage, 0, byteImage.Length).Result;
            return image;
        }

        public override int Count
        {
            get
            {
                return voiceHistory.Count;
            }
        }
    }

    class ImagesVoiceProcesedAdapterViewHolder : Java.Lang.Object
    {
        public ImageView imageView1;
        public Android.Support.Design.Widget.FloatingActionButton floatingActionButton1;
        public ImagesVoiceProcesedAdapterViewHolder(View view)
        {
            imageView1 = view.FindViewById<ImageView>(Resource.Id.imageView1);
            floatingActionButton1 = view.FindViewById<Android.Support.Design.Widget.FloatingActionButton>(Resource.Id.floatingActionButton1);
        }
    }
}