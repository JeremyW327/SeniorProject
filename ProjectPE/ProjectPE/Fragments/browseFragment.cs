using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Runtime;
using System.Collections.Generic;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Widget;
using System;
using ProjectPE.Resources.DataHelper;

namespace ProjectPE.Fragments
{
    public class browseFragment : Fragment
    {
        
        //access to database
        DataBase db = new DataBase();
        List<string> targetAreas = new List<string>();        

        //Implementing RecyclerView and ImageAlbum for
        //Browse Muscle group list
        RecyclerView browseRecyclerView;
        RecyclerView.LayoutManager browseLayoutManager;
        ImageAlbumAdapter imageAdapter;
        ImageAlbum imageAlbum;
        

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);            
        }

        public static browseFragment NewInstance()
        {
            var browseFragment = new browseFragment { Arguments = new Bundle() };
            return browseFragment;            
        }

        
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            
            View view = inflater.Inflate(Resource.Layout.BrowseFragment, null);
            ((AppCompatActivity)Activity).SupportActionBar.SetTitle(Resource.String.frag3Title);
            var ignored = base.OnCreateView(inflater, container, savedInstanceState);        

            //make a new imageAlbum
            imageAlbum = new ImageAlbum();
            targetAreas = db.getCategories();
            //setup recyclerview and adapters
            browseRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.recyclerView);

            //browseLayoutManager = new LinearLayoutManager(Activity);

            browseLayoutManager = new GridLayoutManager(Activity, 2);
            browseRecyclerView.SetLayoutManager(browseLayoutManager);

            imageAdapter = new ImageAlbumAdapter(targetAreas, Activity);

            imageAdapter.ItemClick += OnItemClick;

            browseRecyclerView.SetAdapter(imageAdapter);

            return view;
        }

        private void OnItemClick(object sender, int e)
        {
            LoadPlaylist(e);
        }
   
        void LoadPlaylist(int id)
        {
            Bundle nameBundle = new Bundle();
            nameBundle.PutString("targetAreaName", targetAreas[id]);
            Android.Support.V4.App.Fragment exerciseListFragment = null;

            exerciseListFragment = new exerciseListFragment();

            var ft = FragmentManager.BeginTransaction();
            ft.Replace(Resource.Id.content_frame, exerciseListFragment).AddToBackStack(id.ToString());
            ft.Commit();

            exerciseListFragment.Arguments = nameBundle;
        }
    
        //ImageViewHolder Implementation
        public class ImageViewHolder : RecyclerView.ViewHolder
        {
            public ImageView image { get; private set; }
            public TextView caption { get; private set; }
            public Button button { get; private set; }


            public ImageViewHolder(View itemView, Action<int> listener) : base(itemView)
            {
                //image = itemView.FindViewById<ImageView>(Resource.Id.imageView);
                //caption = itemView.FindViewById<TextView>(Resource.Id.textView);
                button = itemView.FindViewById<Button>(Resource.Id.btnBrowse);
                itemView.Click += (sender, e) => listener(base.LayoutPosition);
            }
        }

        //Adapter for the recyclerview
        public class ImageAlbumAdapter : RecyclerView.Adapter
        {
            public event EventHandler<int> ItemClick;
            List<string> targetArea;
            public ImageAlbum imageAlbum;
            Android.Support.V4.App.FragmentActivity activity;

            //Random number for image selection
            Random randomNum = new Random();

            public override int ItemCount
            {
                get { return targetArea.Count; }
            }

            public ImageAlbumAdapter(List<string> targetArea, Android.Support.V4.App.FragmentActivity activity)
            {
                this.targetArea = targetArea;
                this.activity = activity;
            }

            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                ImageViewHolder viewHolder = holder as ImageViewHolder;

                //switch based on random number to pick the background color
                //TODO: Move GetImageNume to own function to clean up code
                switch (randomNum.Next(1, 6))
                {
                    case 1:
                        viewHolder.button.SetBackgroundResource(Resource.Drawable.BlueBackground);
                        break;
                    case 2:
                        viewHolder.button.SetBackgroundResource(Resource.Drawable.GreenBackground);
                        break;
                    case 3:
                        viewHolder.button.SetBackgroundResource(Resource.Drawable.RedBackground);
                        break;
                    case 4:
                        viewHolder.button.SetBackgroundResource(Resource.Drawable.YellowBackground);
                        break;
                    case 5:
                        viewHolder.button.SetBackgroundResource(Resource.Drawable.PurpleBackground);
                        break;
                    case 6:
                        viewHolder.button.SetBackgroundResource(Resource.Drawable.OrangeBackground);
                        break;
                    default:
                        viewHolder.button.SetBackgroundResource(Resource.Drawable.BlueBackground);
                        break;
                }
                //viewHolder.button.SetBackgroundResource(Resource.Drawable.BlueBackground);
                viewHolder.button.Text = targetArea[position];
            }

            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.BrowseFragRow, parent, false);
                ImageViewHolder viewHolder = new ImageViewHolder(itemView, OnClick);

                viewHolder.button.Click += (sender, e) =>
                {
                    var pos = viewHolder.AdapterPosition;

                    Bundle nameBundle = new Bundle();
                    string name = targetArea[pos];
                    nameBundle.PutString("targetAreaName", name);
                    Android.Support.V4.App.Fragment exerciseListFragment = null;
                    exerciseListFragment = new exerciseListFragment();
                    var ft = activity.SupportFragmentManager.BeginTransaction();

                    //var ft = ((AppCompatActivity).Context).getSupportFragmentManager();
                    ft.Replace(Resource.Id.content_frame, exerciseListFragment).AddToBackStack(pos.ToString());
                    ft.Commit();

                    exerciseListFragment.Arguments = nameBundle;
                };
                
                return viewHolder;
            }

            void OnClick (int position)
            {
                if (ItemClick != null)
                    ItemClick(this, position);
            }                                 
        }
    }
}