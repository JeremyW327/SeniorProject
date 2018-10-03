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

namespace ProjectPE
{
    class PlaylistsAdapter : ArrayAdapter<(string, int)>
    {
        Activity context;
        public PlaylistsAdapter(Activity context, IList<(string, int)> objects)
            : base(context, Android.Resource.Id.Text1, objects)
        {
            this.context = context;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem2, null);

            var item = GetItem(position);

            view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = item.Item1;
            view.FindViewById<TextView>(Android.Resource.Id.Text2).Text = item.Item2.ToString() + " exercises";

            return view;
        }
    }

    class ExerciseAdapter : ArrayAdapter<(string, int, int)>
    {
        Activity context;
        public ExerciseAdapter(Activity context, IList<(string, int, int)> objects)
            : base(context, Android.Resource.Id.Text1, objects)
        {
            this.context = context;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem2, null);

            var item = GetItem(position);

            view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = item.Item1;
            view.FindViewById<TextView>(Android.Resource.Id.Text2).Text = "Sets: " + item.Item2.ToString() + "  " + "Reps: " + item.Item3.ToString();

            return view;
        }
    }
}