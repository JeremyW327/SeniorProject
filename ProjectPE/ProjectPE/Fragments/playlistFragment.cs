using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using System.Collections.Generic;



using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Widget;
using ProjectPE.Resources.DataHelper;
using ProjectPE.Resources.Model;
using Android.Support.V7.Widget;
using System;
using Android.Content;

namespace ProjectPE.Fragments
{
    public class playlistFragment : Fragment
    {
        //store name of playlist 
        string titleName;
        //access to database and exercise class
        DataBase db = new DataBase();        
        List<dbExercise> exercises = new List<dbExercise>();

        RecyclerView recyclerView;
        RecyclerView.LayoutManager LayoutManager;
        RecyclerAdapter adapter;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);              
        }

        public static playlistFragment NewInstance()
        {
            var playlistFragment = new playlistFragment { Arguments = new Bundle() };
            return playlistFragment;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            HasOptionsMenu = true;
            titleName = Arguments.GetString("playlistName");
            View view = inflater.Inflate(Resource.Layout.PlaylistsFrag, null);            
            ((AppCompatActivity)Activity).SupportActionBar.Title = titleName;
            var ignored = base.OnCreateView(inflater, container, savedInstanceState);

            //get the correct exercises from the name of the playlist
            //that was passed in
            exercises = db.getPlaylistExercises(titleName);

            //set recyclerview layout
            recyclerView = view.FindViewById<RecyclerView>(Resource.Id.recyclerViewPlaylistFrag);

            //set layout manager
            LayoutManager = new LinearLayoutManager(Activity);
            recyclerView.SetLayoutManager(LayoutManager);

            //load data into adapter
            adapter = new RecyclerAdapter(exercises, titleName);

            recyclerView.SetAdapter(adapter);
            return view;
        }

        //override options menu and add way to add exercise to playlist
        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.PlaylistAddExercises, menu);
            base.OnCreateOptionsMenu(menu, inflater);
        }

        //function for options menu. Updating playlists name and adding
        //exercise(s) to playlist
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
           
            View view = LayoutInflater.Inflate(Resource.Layout.EditPlaylistDialog, null);
            AlertDialog.Builder builder = new AlertDialog.Builder(Context);
            builder.SetView(view);
            AlertDialog alertDialog = builder.Create();

            Button updateButton = view.FindViewById<Button>(Resource.Id.btnUpdate);
            Button cancelButton = view.FindViewById<Button>(Resource.Id.btnCancel);
            EditText txtName = view.FindViewById<EditText>(Resource.Id.txtPlaylistName);

            updateButton.Click += delegate
            {

                var name = txtName.Text;                
                //verify string is not empty
                if (!string.IsNullOrEmpty(name))
                {
                    //check it see if name is already taken, if not item is inserted into database
                    if (db.updatePlaylistName(titleName, name))
                    {
                        //update the action bar title here instead of reloading the whole page
                        //and update the titleName to the new name incase of another name change without
                        //reloading page
                        ((AppCompatActivity)Activity).SupportActionBar.Title = name;
                        titleName = name;

                        Toast.MakeText(this.Activity, "Playlist Name Updated.", ToastLength.Short).Show();
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
        
        //Implement Recycler adapter that will hold the list of exercises
        public class RecyclerAdapter : RecyclerView.Adapter
        {
            //number for expanding the recyclerview
            private int expandedPosition = -1;
            private string playlistName;
            private List<dbExercise> exercises;
            public event EventHandler<int> ItemClick;
            DataBase dataBase = new DataBase();

            public RecyclerAdapter (List<dbExercise> exerciseList, string playName)
            {
                exercises = exerciseList;
                playlistName = playName;
            }
            public override int ItemCount
            {
                get { return exercises.Count; }
            }           
            
            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                ExerciseListViewHolder viewHolder = holder as ExerciseListViewHolder;                              

                //set exercise name
                viewHolder.txtName.Text = exercises[position].name;

                //set value for reps and sets                
                viewHolder.txtSets.Text = "Sets: " + exercises[position].sets;                
                viewHolder.txtReps.Text = "Reps: " + exercises[position].reps;

                //check value for equipment, if weight is empty get rid of the lbs
                if (exercises[position].weight == "N/A")
                    viewHolder.txtWeight.Text = "Weight: " + exercises[position].weight;
                else
                    viewHolder.txtWeight.Text = "Weight: " + exercises[position].weight + " lbs";

                //set value for target area
                viewHolder.txtEquipment.Text = "Equipment: " + exercises[position].equipment;

                //Comparing position to see if the hidden layout needs
                //to be shown or hidden
                if (position == expandedPosition)
                {
                    viewHolder.expandLayout.Visibility = ViewStates.Visible;
                }
                else
                {
                    viewHolder.expandLayout.Visibility = ViewStates.Gone;
                }                              
            }

            
                

            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ExerciseRow, parent, false);
               // View deleteView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.DeleteItem, null);
                //View editView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.EditExerciseDialog, null);
                ExerciseListViewHolder viewHolder = new ExerciseListViewHolder(itemView, OnClick);
                                
                //click event for delete
                viewHolder.deleteButton.Click += (sender, e) =>
                {
                    var pos = viewHolder.AdapterPosition;                    ;
                    var exerciseName = exercises[pos].name;

                    //make popup menu for confirmation of deleting from database
                    View deleteView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.DeleteItem, null);
                    AlertDialog.Builder builder = new AlertDialog.Builder(parent.Context);
                    builder.SetView(deleteView);
                    AlertDialog alertDialog = builder.Create();

                    Button deleteButton = deleteView.FindViewById<Button>(Resource.Id.btnDelete);
                    Button cancelButton = deleteView.FindViewById<Button>(Resource.Id.btnCancel);
                    TextView txtTitle = deleteView.FindViewById<TextView>(Resource.Id.titleDelete);
                    txtTitle.Text = "Remove exercise from playlist?";

                    deleteButton.Click += delegate
                    {
                        dataBase.removeExerciseFromPlaylist(playlistName, exerciseName);
                        Toast.MakeText(parent.Context, exerciseName + " removed", ToastLength.Short).Show();
                        exercises.RemoveAt(pos);
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

                //click event for edit
                viewHolder.editButton.Click += (sender, e) =>
                {
                    var pos = viewHolder.AdapterPosition; ;
                    var exerciseName = exercises[pos].name;
                    View editView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.EditExerciseDialog, null);

                    AlertDialog.Builder builder = new AlertDialog.Builder(parent.Context);
                    builder.SetView(editView);
                    AlertDialog alertDialog = builder.Create();

                    Button okButton = editView.FindViewById<Button>(Resource.Id.btnCreate);
                    Button cancelButton = editView.FindViewById<Button>(Resource.Id.btnCancel);
                    EditText txtName = editView.FindViewById<EditText>(Resource.Id.txtExerciseName);
                    EditText txtSets = editView.FindViewById<EditText>(Resource.Id.txtSets);
                    EditText txtReps = editView.FindViewById<EditText>(Resource.Id.txtReps);
                    EditText txtWeight = editView.FindViewById<EditText>(Resource.Id.txtWeight);
                    EditText txtEquipment = editView.FindViewById<EditText>(Resource.Id.txtEquipment);
                    EditText txtTargetArea = editView.FindViewById<EditText>(Resource.Id.txtTargetArea);

                    //Put current values of the exercise in the dialog so its easy to see
                    //what they are when the user is editing the exercise
                    //if they have a value of N/A leave them blank so the text hint shows
                    txtName.Text = exercises[pos].name;
                    if (exercises[pos].sets != "0")
                        txtSets.Text = exercises[pos].sets.ToString();
                    if (exercises[pos].reps != "0")
                        txtReps.Text = exercises[pos].reps.ToString();
                    if (exercises[pos].weight != "N/A")
                        txtWeight.Text = exercises[pos].weight;
                    if (exercises[pos].equipment != "N/A")
                        txtEquipment.Text = exercises[pos].equipment;
                    if (exercises[pos].targetArea != "N/A")
                        txtTargetArea.Text = exercises[pos].targetArea;

                    okButton.Click += delegate
                    {
                        //Need a valid name, the rest can be 0 or N/A
                        if (string.IsNullOrEmpty(txtName.Text) )
                            Toast.MakeText(parent.Context, "Please enter a valid name.", ToastLength.Short).Show();
                        if (string.IsNullOrEmpty(txtSets.Text))
                            txtSets.Text = "0";
                        if (string.IsNullOrEmpty(txtReps.Text))
                            txtReps.Text = "0";
                        if (string.IsNullOrEmpty(txtWeight.Text))
                            txtWeight.Text = "N/A";
                        if (string.IsNullOrEmpty(txtEquipment.Text))
                            txtEquipment.Text = "N/A";
                        if (string.IsNullOrEmpty(txtTargetArea.Text))
                            txtTargetArea.Text = "N/A";

                        
                        
                        if(dataBase.updateExercise(exerciseName, txtName.Text, txtSets.Text, txtReps.Text, txtWeight.Text, txtTargetArea.Text, txtEquipment.Text))
                        {
                            Toast.MakeText(parent.Context, "Exercise updated.", ToastLength.Short).Show();
                            
                            //update exercise in recyclerview list to add to the recyclerview list
                            
                            exercises[pos].name = txtName.Text;
                            exercises[pos].sets = txtSets.Text;
                            exercises[pos].reps = txtReps.Text;
                            exercises[pos].weight = txtWeight.Text;
                            exercises[pos].targetArea = txtTargetArea.Text;
                            exercises[pos].equipment = txtEquipment.Text;
                            
                            NotifyDataSetChanged();                           
                            alertDialog.Dismiss();
                            
                            
                        }
                        else
                        {
                            Toast.MakeText(parent.Context, "Name is taken.", ToastLength.Short).Show();
                        }
                    };
                    cancelButton.Click += delegate
                    {
                        alertDialog.Dismiss();
                    };
                    alertDialog.Show();
                };

                //click event for search
                //this will open the youtube app for the user to search
                //for the exercise they clicked on
                viewHolder.searchButton.Click += (sender, e) =>
                {
                    var pos = viewHolder.AdapterPosition;
                    var name = exercises[pos].name;
                    Context context = parent.Context;

                    //start new intent that will open the youtube app
                    //and display search results for specific exercise
                    Intent intent = new Intent(Intent.ActionSearch);
                    intent.SetPackage("com.google.android.youtube");
                    intent.PutExtra("query", name + " how to");
                    intent.SetFlags(ActivityFlags.NewTask);
                    context.StartActivity(intent);

                };
                return viewHolder;
            }
            

            void OnClick(int position)
            {
                if (ItemClick != null)
                    ItemClick(this, position);

                if (expandedPosition >= 0)
                {
                    int prev = expandedPosition;
                    NotifyItemChanged(prev);
                }

                expandedPosition = position;
                NotifyItemChanged(expandedPosition);
            }            
        }

        public class ExerciseListViewHolder : RecyclerView.ViewHolder
        {
            public LinearLayout expandLayout { get; private set; }
            public ImageButton deleteButton { get; private set; }
            public ImageButton editButton { get; private set; }
            public ImageButton searchButton { get; private set; }
            public TextView txtName { get; private set; }
            public TextView txtSets { get; private set; }
            public TextView txtReps { get; private set; }
            public TextView txtEquipment { get; private set; }
            public TextView txtWeight { get; private set; }



            public ExerciseListViewHolder (View itemView, Action<int> listener) : base(itemView)
            {
                expandLayout = itemView.FindViewById<LinearLayout>(Resource.Id.expandLayout);
                deleteButton = itemView.FindViewById<ImageButton>(Resource.Id.btnDelete);
                editButton = itemView.FindViewById<ImageButton>(Resource.Id.btnEdit);
                searchButton = itemView.FindViewById<ImageButton>(Resource.Id.btnWatch);

                txtName = itemView.FindViewById<TextView>(Resource.Id.txtName);
                txtSets = itemView.FindViewById<TextView>(Resource.Id.txtSets);
                txtReps = itemView.FindViewById<TextView>(Resource.Id.txtReps);
                txtEquipment = itemView.FindViewById<TextView>(Resource.Id.txtEquipment);
                txtWeight = itemView.FindViewById<TextView>(Resource.Id.txtWeight);

                itemView.Click += (sender, e) => listener(base.LayoutPosition);

            }
        }
        public void LaunchYoutube(string name)
        {
            
        }

    }
}