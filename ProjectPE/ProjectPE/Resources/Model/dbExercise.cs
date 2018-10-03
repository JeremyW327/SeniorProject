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
    public class dbExercise
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        [Unique]
        public string name { get; set; }

        public string equipment { get; set; }

        public string targetArea { get; set; }

        public string reps { get; set; }
        
        public string sets { get; set; }

        public string weight { get; set; }

        [ManyToMany(typeof(dbPlaylistExercise))]
        public List<dbPlaylist> playlists { get; set; }
        public override string ToString()
        {
            return name;
        }
    }
}