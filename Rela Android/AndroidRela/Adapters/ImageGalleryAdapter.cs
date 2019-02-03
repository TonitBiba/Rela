using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidRela.Models.CheckSimilarity;

namespace AndroidRela.Adapters
{
    public class ImageGalleryAdapter : BaseAdapter
    {
        List<HistoryOfProcesedImages> listImagesHistory;
        Context context;

    public ImageGalleryAdapter(Context context, List<HistoryOfProcesedImages> listImagesHistory)
    {
        this.context = context;
        this.listImagesHistory = listImagesHistory;
    }


    public override Java.Lang.Object GetItem(int position)
    {
        return null;
    }

    public override long GetItemId(int position)
    {
        return 0;
    }

    public override View GetView(int position, View convertView, ViewGroup parent)
    {
        try
        {
            ImageView imageview = new ImageView(context);
            imageview.SetImageBitmap(convertImage(listImagesHistory[position].image));
            imageview.LayoutParameters = new Gallery.LayoutParams(500, 450);
            imageview.SetScaleType(ImageView.ScaleType.FitCenter);
            return imageview;
        }
        catch (Exception ex)
        {
            return null;
        }
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
            return listImagesHistory.Count;
        }
    }
}

class ImageGalleryAdapterViewHolder : Java.Lang.Object
{
}
}