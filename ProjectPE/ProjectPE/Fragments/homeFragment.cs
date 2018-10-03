using Android.OS;
using Android.Support.V4.App;
using Android.Views;

using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using System;
using Android.Support.V7.App;
using Android.Widget;
using ProjectPE.Fragments;
using System.Collections.Generic;
using Android.Content;
using ProjectPE.Resources.DataHelper;

namespace ProjectPE
{   
    public class homeFragment : Fragment
    {
        //Recyclerview and adapters
        RecyclerView homeRecyclerView;
        RecyclerView.LayoutManager homeLayoutManager;
        FavoritesAlbumAdapter nameAdapter;        

        FavoriteImages favoriteImages;
        
        DataBase db = new DataBase();
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static homeFragment NewInstance()
        {
            var frag1 = new homeFragment { Arguments = new Bundle() };
            return frag1;
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.HomeFragment, null);
            ((AppCompatActivity)Activity).SupportActionBar.SetTitle(Resource.String.frag1Title);
            var ignored = base.OnCreateView(inflater, container, savedInstanceState);

            //Get the data needed to show the favorite playlists
            favoriteImages = new FavoriteImages();           
           
            Favorites[] favorites = { new Favorites { itemId = Resource.Drawable.ic_playlist_play_black_48dp, favoriteNames = db.getPlaylistFavorites(), sectionTitle = "Favorite Playlists" },
                                      new Favorites { itemId = Resource.Drawable.ic_playlist_play_black_48dp, favoriteNames = db.getUpperBodyPlaylists(), sectionTitle = "Upper Body Playlists" },
                                      new Favorites { itemId = Resource.Drawable.ic_playlist_play_black_48dp, favoriteNames = db.getLowerBodyPlaylists(), sectionTitle = "Lower Body Playlists" } };

            favoriteImages.setFavorites(favorites);
            //Get the recyclerview layout
            homeRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.recyclerView);

            //setup the layout manager
            homeLayoutManager = new LinearLayoutManager(Activity);
            homeRecyclerView.SetLayoutManager(homeLayoutManager);

            //setup the favorite Album adapter
            nameAdapter = new FavoritesAlbumAdapter(favoriteImages, Activity);
            
            //register item click handler
            nameAdapter.ItemClick += OnItemClick;
           
            homeRecyclerView.SetAdapter(nameAdapter);   
            
            
            return view;
        }

        private void OnItemClick(object sender, int e)
        {
            Android.Support.V4.App.Fragment playlistFragment = null;
            playlistFragment = new playlistFragment();
            var ft = FragmentManager.BeginTransaction();
        }
    }

    //Setup the recyclerview and its adapters 
    //to display the favorites
    //ImageViewHolder Implementation
    public class ImageViewHolder : RecyclerView.ViewHolder
    {
        public ImageView image { get; private set; }
        public TextView caption { get; private set; }        

        public ImageViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            image = itemView.FindViewById<ImageView>(Resource.Id.imageCellView);
            caption = itemView.FindViewById<TextView>(Resource.Id.textCellView);            

            itemView.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }

    //Adapter for the recyclerview
    public class FavoriteNameAdapter : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        Random randomNum = new Random();
        public List<string> favorites;
        public int imageId;
        Android.Support.V4.App.FragmentActivity activity;

        public override int ItemCount
        {
            get { return favorites.Count; }
        }

        public FavoriteNameAdapter(Favorites favoriteNamesTemp, Android.Support.V4.App.FragmentActivity activity)
        {
            favorites = favoriteNamesTemp.favoriteNames;
            imageId = favoriteNamesTemp.itemId;
            this.activity = activity;
            
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ImageViewHolder viewHolder = holder as ImageViewHolder;

            if (viewHolder != null)
            { 
                //viewHolder.image.SetImageResource(imageId);

                switch (randomNum.Next(1, 6))
                {
                    case 1:
                        viewHolder.image.SetImageResource(Resource.Drawable.BlueBackground);
                        break;
                    case 2:
                        viewHolder.image.SetImageResource(Resource.Drawable.GreenBackground);
                        break;
                    case 3:
                        viewHolder.image.SetImageResource(Resource.Drawable.RedBackground);
                        break;
                    case 4:
                        viewHolder.image.SetImageResource(Resource.Drawable.YellowBackground);
                        break;
                    case 5:
                        viewHolder.image.SetImageResource(Resource.Drawable.PurpleBackground);
                        break;
                    case 6:
                        viewHolder.image.SetImageResource(Resource.Drawable.OrangeBackground);
                        break;
                    default:
                        viewHolder.image.SetImageResource(Resource.Drawable.BlueBackground);
                        break;
                }
                        

                        viewHolder.caption.Text = favorites[position];
            }

            
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.HomeFragImageCell, parent, false);

            ImageViewHolder viewHolder = new ImageViewHolder(itemView, OnClick);
            

            viewHolder.image.Click += (sender, e) =>
            {
                var pos = viewHolder.AdapterPosition;
                
                Bundle nameBundle = new Bundle();
                string name = favorites[pos];
                nameBundle.PutString("playlistName", name);
                Android.Support.V4.App.Fragment playlistFragment = null;
                playlistFragment = new playlistFragment();
                var ft = activity.SupportFragmentManager.BeginTransaction();
                
                //var ft = ((AppCompatActivity).Context).getSupportFragmentManager();
                ft.Replace(Resource.Id.content_frame, playlistFragment).AddToBackStack(pos.ToString());
                ft.Commit();

                playlistFragment.Arguments = nameBundle;
            };


            return viewHolder;
        }

        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }
    }

    //Implementation for the Viewholder for the sub recyclerview
    public class FavoritesAblumViewHolder : RecyclerView.ViewHolder
    {
        public TextView caption { get; private set; }
        public RecyclerView subRecyclerView { get; private set; }

        public FavoritesAblumViewHolder(View itemView, Action<int> listener) : base (itemView)
        {
            subRecyclerView = itemView.FindViewById<RecyclerView>(Resource.Id.recyclerViewHome);

            var layoutManager = new LinearLayoutManager(itemView.Context)
            {
                Orientation = (int)Orientation.Horizontal
            };

            subRecyclerView.SetLayoutManager(layoutManager);

            caption = itemView.FindViewById<TextView>(Resource.Id.homeSectionTitle);

            itemView.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }

    //Implementation for the Adapter for the sub recyclerview
    public class FavoritesAlbumAdapter : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        Android.Support.V4.App.FragmentActivity activity;

        //Data for favorite exercises and playlists
        public FavoriteImages favoriteImages;        

        public FavoritesAlbumAdapter (FavoriteImages favImages, Android.Support.V4.App.FragmentActivity activity)
        {
            favoriteImages = favImages;
            this.activity = activity;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            FavoritesAblumViewHolder viewHolder = holder as FavoritesAblumViewHolder;
            var favAdapter = new FavoriteNameAdapter(favoriteImages[position], activity);
            viewHolder.subRecyclerView.SetAdapter(favAdapter);
            viewHolder.caption.Text = favoriteImages[position].sectionTitle;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.HomeFragRow, parent, false);
            FavoritesAblumViewHolder viewHolder = new FavoritesAblumViewHolder(itemView, OnClick);
            return viewHolder;
        }
        public override int ItemCount
        {
            get { return favoriteImages.numFavorites; }
        }

        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }
    }    
}