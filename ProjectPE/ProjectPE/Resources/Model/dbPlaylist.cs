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
    public class dbPlaylist
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        [Unique]
        public string name { get; set; }

        public bool favorite { get; set; }

        public string playlistType { get; set; }

        [ManyToMany(typeof(dbPlaylistExercise))]
        public List<dbExercise> exercises { get; set; }

        public override string ToString()
        {
            return name;
        }
    }
}