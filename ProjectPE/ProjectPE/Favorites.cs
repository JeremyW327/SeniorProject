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
using ProjectPE.Resources.DataHelper;

namespace ProjectPE
{
        
    //For the Home fragment. Stores the names of the 
    //exercises and playlists that are mark favorite
    public class Favorites
    {
        public string sectionTitle;
        public List<string> favoriteNames;
        public int itemId;
        

        //public Favorites(String title, List<string> names)
        //{
        //    sectionTitle = title;
        //    favoriteNames = names;
        //}

        public string getTitle
        {
            get{ return sectionTitle; }
        }

        public List<string> getNames
        {
            get { return favoriteNames; }
        } 
        
        public int getItemId
        {
            get { return itemId; }
        }
    }

    //For the home fragment. This will pull data from the database
    //to store in the Favorites class and combine it with any images
    //associated with those favorites
    public class FavoriteImages
    {
        private Favorites[] favorites;
       

        static public List<string> getPlaylistFavorites()
        {
            DataBase db = new DataBase();
            var tempList = db.getPlaylistFavorites();
            return tempList;
            
        }
       
        //use built in image / favorite collection
        //possible upgrade to image database in future
        static Favorites[] builtInFavorites =
        {
            new Favorites { itemId = Resource.Drawable.ic_playlist_play_black_48dp, favoriteNames = getPlaylistFavorites(), sectionTitle = "Favorite Playlists" },
            //new Favorites { itemId = Resource.Drawable.ic_fitness_center_black_48dp, favoriteNames = getExerciseFavorites(), sectionTitle = "Favorite Exercises" }
        };

        public FavoriteImages()
        {
            favorites = builtInFavorites;
        }

        public int numFavorites
        {
            get { return favorites.Length; }
        }

        //For indexing only
        public Favorites this[int i]
        {
            get { return favorites[i]; }
        }

        public List<string> getNames (int i)
        {
            return favorites[i].favoriteNames;
        }

        public void setFavorites (Favorites[] favorites)
        {
            this.favorites = favorites;
        }
    }
}