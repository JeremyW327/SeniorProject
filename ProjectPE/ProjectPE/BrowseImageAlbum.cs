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
using Android.Support.V7.Widget;


namespace ProjectPE
{
    public class Image
    {
        public int ImageId;
        public string MuscleGroup;

        public int getPhotoId
        {
            get { return ImageId; }
        }

        public string getMuscleGroup
        {
            get { return MuscleGroup; }
        }     
    }

    public class ImageAlbum
    {
        static Image[] preLoadedImages = {
            new Image { ImageId = Resource.Drawable.Arms, MuscleGroup = "Arms"},
            new Image { ImageId = Resource.Drawable.Back, MuscleGroup = "Back"},
            new Image { ImageId = Resource.Drawable.Legs, MuscleGroup = "Legs"},
            new Image { ImageId = Resource.Drawable.Chest, MuscleGroup = "Chest"},
            new Image { ImageId = Resource.Drawable.Shoulders, MuscleGroup = "Shoulders"},
            new Image { ImageId = Resource.Drawable.Abs, MuscleGroup = "Abs"},
        };

        private Image[] imageAlbum;

        public ImageAlbum()
        {
            imageAlbum = preLoadedImages;
        }

        public int ImageCount
        {
            get { return imageAlbum.Length; }
        }

        //To be able to index the Image Album
        public Image this[int i]
        {
            get { return imageAlbum[i]; }
        }
    }
}