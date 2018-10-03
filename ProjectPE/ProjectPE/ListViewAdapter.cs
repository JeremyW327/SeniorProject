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
using ProjectPE.Resources.Model;
using Android.Graphics;

namespace ProjectPE
{
    class ListViewAdapter : ArrayAdapter<string>
    {        
        string itemName;
        Activity context;
        public ListViewAdapter(Activity context, IList<string> objects)
            : base(context, Android.Resource.Id.Text1, objects)
        {
            this.context = context;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem1, null);

            itemName = GetItem(position);          
            
            view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = itemName;
            view.FindViewById<TextView>(Android.Resource.Id.Text1).SetTextColor(Color.ParseColor("#FAFAFA"));
            view.FindViewById<TextView>(Android.Resource.Id.Text1).TextAlignment = TextAlignment.Center;

            return view;
        }               
    } 
}
    