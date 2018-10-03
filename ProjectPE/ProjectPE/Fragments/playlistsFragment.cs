using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using System.Collections.Generic;


using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Support.V7.App;
using Android.Widget;
using ProjectPE.Fragments;
using ProjectPE.Resources.Model;
using ProjectPE.Resources.DataHelper;
using SQLite;
using System;

namespace ProjectPE.Fragments
{
    public class playlistsFragment : Fragment
    {       
        //string playlistName;
        List<dbPlaylist> playlists = new List<dbPlaylist>();
        DataBase db = new DataBase();

        //Implement Recyclerview and RV adapter
        RecyclerView recyclerView;
        RecyclerView.LayoutManager layoutManager;
        RecyclerAdapter adapter;        

        public override void OnCreate(Bundle savedInstanceState)
        {            
            base.OnCreate(savedInstanceState);
        }

        public static playlistsFragment NewInstance()
        {
            var frag2 = new playlistsFragment { Arguments = new Bundle() };
            return frag2;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            HasOptionsMenu = true;
            View view = inflater.Inflate(Resource.Layout.PlaylistsFrag, null);
            ((AppCompatActivity)Activity).SupportActionBar.SetTitle(Resource.String.frag2Title); 
            var ignored = base.OnCreateView(inflater, container, savedInstanceState);

            //Pull the list of playlists from the database
            playlists = db.selectTablePlaylists();            

            //set recyclerview layout
            recyclerView = view.FindViewById<RecyclerView>(Resource.Id.recyclerViewPlaylistFrag);

            //set layout manager
            layoutManager = new LinearLayoutManager(Activity);
            recyclerView.SetLayoutManager(layoutManager);

            //load data into adapter
            adapter = new RecyclerAdapter(playlists);

            adapter.ItemClick += OnItemClick;

            recyclerView.SetAdapter(adapter);
            
            return view;
        }

        private void OnItemClick(object sender, int e)
        {
            //int nameNum = e + 1;            
            
            //Toast.MakeText(Context, "This is name number " + playlists[e].name, ToastLength.Short).Show();
            LoadPlaylist(e);
        }

        //override create options menu to add the Add playlist function
        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.PlaylistsMenu, menu);
            base.OnCreateOptionsMenu(menu, inflater);
        }        

        //function for what action to take when use clicks add playlist
        //in options menu       

        public override bool OnOptionsItemSelected(IMenuItem item)
        {          
            View view = LayoutInflater.Inflate(Resource.Layout.PlaylistDialog, null);
            AlertDialog.Builder builder = new AlertDialog.Builder(Context);
            builder.SetView(view);
            AlertDialog alertDialog = builder.Create();
            
            Button createButton = view.FindViewById<Button>(Resource.Id.btnCreate);
            Button cancelButton = view.FindViewById<Button>(Resource.Id.btnCancel);
            EditText txtName = view.FindViewById<EditText>(Resource.Id.txtPlaylistName);

            createButton.Click += delegate
            {
                
                var name = txtName.Text;                
                //alertDialog.Dismiss();
                //verify string is not empty
                if (!string.IsNullOrEmpty(name))
                {
                    //if not empty create new playlist
                    dbPlaylist newPlaylist = new dbPlaylist { name = name };                    
                    //check it see if name is already taken, if not insert into database
                    if (db.InsertIntoTablePlaylist(newPlaylist))
                    {
                        Toast.MakeText(this.Activity, name + " created.", ToastLength.Short).Show();
                        //playlists.Add(newPlaylist);
                        //adapter.NotifyItemInserted(playlists.Count - 1);
                        adapter.addItem(newPlaylist, adapter.ItemCount);
                        
                        alertDialog.Dismiss();
                    }
                    else
                    {
                        Toast.MakeText(this.Activity, "Name is taken.", ToastLength.Short).Show();
                    }
                }
                else
                {                    
                    Toast.MakeText(this.Activity, "Please enter a valid name.", ToastLength.Short).Show();
                }

            };
            cancelButton.Click += delegate
            {              
                alertDialog.Dismiss();
            };
            alertDialog.Show();
            
            return true;
        }   
        void LoadPlaylist(int id)
        {
            //create playlist fragment and pass name into bundle
            Bundle nameBundle = new Bundle();
            string name = playlists[id].name;
            nameBundle.PutString("playlistName", name);
            Android.Support.V4.App.Fragment playlistFragment = null;

            playlistFragment = new playlistFragment();

            var ft = FragmentManager.BeginTransaction();
            ft.Replace(Resource.Id.content_frame, playlistFragment).AddToBackStack(id.ToString());
            ft.Commit();

            playlistFragment.Arguments = nameBundle;
        }

        public class RecyclerAdapter : RecyclerView.Adapter
        {
            private List<dbPlaylist> playlists;
            public event EventHandler<int> ItemClick;
            DataBase dataBase = new DataBase();
            
            public RecyclerAdapter (List<dbPlaylist> playlist)
            {
                playlists = playlist;
            }
            public override int ItemCount
            {
                get { return playlists.Count; }
            }

            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                PlaylistViewHolder viewHolder = holder as PlaylistViewHolder;

                if (playlists[position].favorite == true)
                {
                    viewHolder.imageButton.SetImageResource(Resource.Drawable.ic_star_green_A700_18dp);   
                }
                else
                {
                    viewHolder.imageButton.SetImageResource(Resource.Drawable.ic_star_border_green_A700_18dp);

                }

                viewHolder.txtName.Text = playlists[position].name;

                //check if exercises is null 
                //For newly created playlists
                if (playlists[position].exercises == null)
                {
                    viewHolder.txtDescription.Text = "0 exercises";
                }
                else
                {
                    viewHolder.txtDescription.Text = playlists[position].exercises.Count + " exercises";
                }
                    
                
            }

            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.PlaylistExerciseRow2, parent, false);
                View deleteView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.DeleteItem, null);
                PlaylistViewHolder viewHolder = new PlaylistViewHolder(itemView, OnClick);
                
                //click event for the favorite button
                viewHolder.imageButton.Click += (sender, e) =>
                {
                    var pos = viewHolder.AdapterPosition;
                    var name = playlists[pos].name;

                    if (playlists[pos].favorite == true)
                    {
                        //set favorite to false and update list
                        Toast.MakeText(parent.Context, "Removed from favorites", ToastLength.Short).Show();
                        viewHolder.imageButton.SetImageResource(Resource.Drawable.ic_star_border_green_A700_18dp);
                        dataBase.updateFavorite(name, false);
                        playlists[pos].favorite = false;

                    }
                    else
                    {
                        //set favorite to true and update list
                        Toast.MakeText(parent.Context, "Added to favorites", ToastLength.Short).Show();
                        viewHolder.imageButton.SetImageResource(Resource.Drawable.ic_star_green_A700_18dp);
                        dataBase.updateFavorite(name, true);
                        playlists[pos].favorite = true;
                    }
                    
                };

                //click event for the delete button                
              
                viewHolder.deleteButton.Click += (sender, e) =>
                {
                    var pos = viewHolder.AdapterPosition;
                    var name = playlists[pos].name;

                    //make popup menu for confirmation of deleting from database
                    AlertDialog.Builder builder = new AlertDialog.Builder(parent.Context);
                    builder.SetView(deleteView);
                    AlertDialog alertDialog = builder.Create();

                    Button deleteButton = deleteView.FindViewById<Button>(Resource.Id.btnDelete);
                    Button cancelButton = deleteView.FindViewById<Button>(Resource.Id.btnCancel);
                    TextView txtTitle = deleteView.FindViewById<TextView>(Resource.Id.titleDelete);
                    txtTitle.Text = "Delete Playlist?";

                    deleteButton.Click += delegate
                    {
                        dataBase.deletePlaylist(name);
                        Toast.MakeText(parent.Context, name + " deleted", ToastLength.Short).Show();
                        playlists.RemoveAt(pos);
                        NotifyDataSetChanged();
                        alertDialog.Dismiss();
                    };
                    cancelButton.Click += delegate
                    {
                        //close window
                        alertDialog.Dismiss();
                    };
                    alertDialog.Show();                    
                };

                return viewHolder;
            }

            void OnClick(int position)
            {
                if (ItemClick != null)
                    ItemClick(this, position);
            }

            public void addItem(dbPlaylist playlist, int index)
            {
                playlists.Add(playlist);
                NotifyItemInserted(index);
            }

        }
        
        public class PlaylistViewHolder : RecyclerView.ViewHolder
        {
            public ImageButton imageButton { get; private set; }
            public TextView txtName { get; private set; }
            public TextView txtDescription { get; private set; }
            public ImageButton deleteButton { get; private set; }
            

            public PlaylistViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
                imageButton = itemView.FindViewById<ImageButton>(Resource.Id.imageButton1);
                deleteButton = itemView.FindViewById<ImageButton>(Resource.Id.imageButton2);
                txtName = itemView.FindViewById<TextView>(Resource.Id.txtName);
                txtDescription = itemView.FindViewById<TextView>(Resource.Id.txtDescription);

                itemView.Click += (sender, e) => listener(base.LayoutPosition);
            }
        }     
    }
}




