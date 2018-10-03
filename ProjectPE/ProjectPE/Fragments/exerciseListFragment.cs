using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using ProjectPE.Resources.DataHelper;
using ProjectPE.Resources.Model;
using Android.Support.V7.Widget;

namespace ProjectPE.Fragments
{
    public class exerciseListFragment : Android.Support.V4.App.Fragment
    {
        //database access
        DataBase db = new DataBase();       
        List<dbExercise> exercises = new List<dbExercise>();
        List<string> nameList = new List<string>();

        //recyclerview adapater and layout manager
        RecyclerView recyclerView;
        RecyclerView.LayoutManager LayoutManager;
        RecyclerAdapter adapter;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static exerciseListFragment NewInstance()
        {
            var exerciseListFragment = new exerciseListFragment { Arguments = new Bundle() };
            return exerciseListFragment;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            HasOptionsMenu = true;
            string titleName = Arguments.GetString("targetAreaName");                       
            View view = inflater.Inflate(Resource.Layout.TargetAreaFragment, null);            
            ((AppCompatActivity)Activity).SupportActionBar.Title = titleName;
            var ignored = base.OnCreateView(inflater, container, savedInstanceState);         

            //get the list of exercises that matches the target area
            exercises = db.getTargetAreaExercises(titleName);
            nameList = getPlaylistNames(db.selectTablePlaylists());

            //set recyclerview layout
            recyclerView = view.FindViewById<RecyclerView>(Resource.Id.recyclerViewSearchFrag);

            //set layout manager
            LayoutManager = new LinearLayoutManager(Activity);
            recyclerView.SetLayoutManager(LayoutManager);

            //load data into adapter
            adapter = new RecyclerAdapter(exercises, nameList, this.Activity);

            //adapter.ItemClick += OnItemClick;

            recyclerView.SetAdapter(adapter);

            return view;
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.SearchFragmentMenu, menu);
            base.OnCreateOptionsMenu(menu, inflater);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            //Setup the view and click event for adding an exercise to the database
            //reuse the axml from editExercise
            View view = LayoutInflater.Inflate(Resource.Layout.EditExerciseDialog, null);

            Android.Support.V7.App.AlertDialog.Builder builder = new Android.Support.V7.App.AlertDialog.Builder(Context);
            builder.SetView(view);
            Android.Support.V7.App.AlertDialog alertDialog = builder.Create();

            Button okButton = view.FindViewById<Button>(Resource.Id.btnCreate);
            Button cancelButton = view.FindViewById<Button>(Resource.Id.btnCancel);
            EditText txtName = view.FindViewById<EditText>(Resource.Id.txtExerciseName);
            EditText txtSets = view.FindViewById<EditText>(Resource.Id.txtSets);
            EditText txtReps = view.FindViewById<EditText>(Resource.Id.txtReps);
            EditText txtWeight = view.FindViewById<EditText>(Resource.Id.txtWeight);
            EditText txtEquipment = view.FindViewById<EditText>(Resource.Id.txtEquipment);
            EditText txtTargetArea = view.FindViewById<EditText>(Resource.Id.txtTargetArea);
            TextView txtTitle = view.FindViewById<TextView>(Resource.Id.titlePlaylist);

            txtTitle.Text = "Create Exercise";

            okButton.Click += delegate
            {
                //Need a valid name, the rest can be 0 or N/A
                if (string.IsNullOrEmpty(txtName.Text))
                    Toast.MakeText(Context, "Please enter a valid name.", ToastLength.Short).Show();
                if (string.IsNullOrEmpty(txtSets.Text))
                    txtSets.Text = "0";
                if (string.IsNullOrEmpty(txtReps.Text))
                    txtReps.Text = "0";
                if (string.IsNullOrEmpty(txtWeight.Text))
                    txtWeight.Text = "N/A";
                if (string.IsNullOrEmpty(txtEquipment.Text))
                    txtEquipment.Text = "N/A";
                if (string.IsNullOrEmpty(txtTargetArea.Text))
                    txtTargetArea.Text = "Other";

                dbExercise newExercise = new dbExercise
                {
                    name = txtName.Text,
                    sets = txtSets.Text,
                    reps = txtReps.Text,
                    weight = txtWeight.Text,
                    equipment = txtEquipment.Text,
                    targetArea = txtTargetArea.Text
                };

                if (db.InsertIntoTableExercise(newExercise))
                {
                    Toast.MakeText(Context, "Exercise added.", ToastLength.Short).Show();

                    //update exercise in recyclerview list to add to the recyclerview list                  

                    adapter.addItem(newExercise, adapter.ItemCount);
                    alertDialog.Dismiss();
                }
                else
                {
                    Toast.MakeText(Context, "Name is taken.", ToastLength.Short).Show();
                }
            };
            cancelButton.Click += delegate
            {
                alertDialog.Dismiss();
            };
            alertDialog.Show();
            return true;
        }

        public class RecyclerAdapter : RecyclerView.Adapter
        {
            private List<dbExercise> exercises;            
            private List<string> playlistNames;
            private string targetArea;

            public event EventHandler<int> ItemClick;
            DataBase dataBase = new DataBase();
            private readonly Activity activity;            

            public RecyclerAdapter(List<dbExercise> exercises, List<string> nameList, Activity activity)
            {
                this.exercises = exercises;
                this.activity = activity;
                playlistNames = nameList;
                targetArea = exercises[0].targetArea;
      
            }
            public override int ItemCount { get { return exercises.Count; } }

            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                ExerciseSearchViewHolder viewHolder = holder as ExerciseSearchViewHolder;

                viewHolder.txtName.Text = exercises[position].name;
                viewHolder.txtTargetArea.Text = exercises[position].targetArea;
            }

            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.SearchFragmentRow, parent, false);
                ExerciseSearchViewHolder viewHolder = new ExerciseSearchViewHolder(itemView, OnClick);

                viewHolder.addButton.Click += (sender, e) =>
                {
                    //adding a exercise to a playlist
                    var pos = viewHolder.AdapterPosition;
                    var exerciseName = exercises[pos].name;



                    View addView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.SearchListViewDialog, null);

                    Android.Support.V7.App.AlertDialog.Builder builder = new Android.Support.V7.App.AlertDialog.Builder(parent.Context);
                    builder.SetView(addView);
                    Android.Support.V7.App.AlertDialog alertDialog = builder.Create();

                    ListView listView = addView.FindViewById<ListView>(Resource.Id.searchLV);
                    ListViewAdapter adapter = new ListViewAdapter(activity, playlistNames);


                    listView.Adapter = adapter;

                    listView.ItemClick += (s, item) =>
                    {
                        var tempName = listView.GetItemAtPosition(item.Position).ToString();

                        dataBase.addExerciseFromPlaylist(tempName, exerciseName);
                        alertDialog.Dismiss();
                        Toast.MakeText(parent.Context, "Exercise Added.", ToastLength.Short).Show();
                    };

                    alertDialog.Show();



                };

                viewHolder.deleteButton.Click += (sender, e) =>
                {
                    var pos = viewHolder.AdapterPosition;
                    var name = exercises[pos].name;

                    //make popup menu for confirmation of deleting from database
                    View deleteView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.DeleteItem, null);
                    Android.Support.V7.App.AlertDialog.Builder builder = new Android.Support.V7.App.AlertDialog.Builder(parent.Context);
                    builder.SetView(deleteView);
                    Android.Support.V7.App.AlertDialog alertDialog = builder.Create();

                    Button deleteButton = deleteView.FindViewById<Button>(Resource.Id.btnDelete);
                    Button cancelButton = deleteView.FindViewById<Button>(Resource.Id.btnCancel);
                    TextView txtTitle = deleteView.FindViewById<TextView>(Resource.Id.titleDelete);
                    txtTitle.Text = "Delete Exercise?";

                    deleteButton.Click += delegate
                    {
                        dataBase.deleteExercise(name);
                        Toast.MakeText(parent.Context, name + " deleted", ToastLength.Short).Show();
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

                return viewHolder;
            }

            void OnClick(int position)
            {
                if (ItemClick != null)
                    ItemClick(this, position);
            }

            public void addItem(dbExercise exercise, int index)
            {
                if (exercise.targetArea == targetArea)
                {
                    exercises.Add(exercise);
                    NotifyItemInserted(index);
                }
            }
        }

        public class ExerciseSearchViewHolder : RecyclerView.ViewHolder
        {
            public ImageButton addButton { get; private set; }
            public ImageButton deleteButton { get; private set; }
            public TextView txtName { get; private set; }
            public TextView txtTargetArea { get; private set; }


            public ExerciseSearchViewHolder(View itemView, Action<int> listener) : base(itemView)
            {
                addButton = itemView.FindViewById<ImageButton>(Resource.Id.btnAddToPlaylist);
                deleteButton = itemView.FindViewById<ImageButton>(Resource.Id.btnDeleteExercise);
                txtName = itemView.FindViewById<TextView>(Resource.Id.txtName);
                txtTargetArea = itemView.FindViewById<TextView>(Resource.Id.txtDescription);

                itemView.Click += (sender, e) => listener(base.LayoutPosition);
            }
        }

        //return a list of just the names of playlists
        public List<string> getPlaylistNames(List<dbPlaylist> playlist)
        {
            List<string> names = new List<string>();
            foreach (var playlistName in playlist)
            {
                names.Add(playlistName.name);
            }
            return names;
        }
    }
}