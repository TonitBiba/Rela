using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace AndroidRela.Adapters
{
    class FillListViewAdapter : BaseAdapter
    {
        private LayoutInflater inflater;
        Context context;
        List<string> userData;

        public FillListViewAdapter(Context context, List<string> userData)
        {
            this.context = context;
            this.userData = userData;
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return userData.IndexOf(userData[position]);
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
                convertView = inflater.Inflate(Resource.Layout.TempleateTextView, parent, false);
            }
            FillListViewAdapterViewHolder holder = new FillListViewAdapterViewHolder(convertView);
            holder.userText.Text = userData[position];

            return convertView;
        }

        //Fill in cound here, currently 0
        public override int Count
        {
            get
            {
                return userData.Count;
            }
        }

    }

    class FillListViewAdapterViewHolder : Java.Lang.Object
    {
        public TextView userText;

        public FillListViewAdapterViewHolder(View view)
        {
            userText = view.FindViewById<TextView>(Resource.Id.textView1);
        }
    }
}