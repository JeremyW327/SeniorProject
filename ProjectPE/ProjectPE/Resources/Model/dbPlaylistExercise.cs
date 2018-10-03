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
using SQLite;
using SQLiteNetExtensions.Attributes;


namespace ProjectPE.Resources.Model
{
    public class dbPlaylistExercise
    {
        [ForeignKey(typeof(dbPlaylist))]
        public int playlistID { get; set; }

        [ForeignKey(typeof(dbExercise))]
        public int exerciseID { get; set; }
    }
}