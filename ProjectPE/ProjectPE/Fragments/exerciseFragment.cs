using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using System.Collections.Generic;


using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Widget;
using ProjectPE.Resources.DataHelper;
using ProjectPE.Resources.Model;

namespace ProjectPE.Fragments
{
    public class exerciseFragment : Fragment
    {
        TextView targetArea;
        TextView repsSets;
        TextView favorite;
        DataBase db = new DataBase();
        dbExercise exercise = new dbExercise();

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);           

            // Create your fragment here
        }

        public static exerciseFragment NewInstance()
        {
            var exerciseFragment = new exerciseFragment { Arguments = new Bundle() };
            return exerciseFragment;
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            string titleName = Arguments.GetString("exerciseName");
            View view = inflater.Inflate(Resource.Layout.exerciseFragment, null);
            //((AppCompatActivity)Activity).SupportActionBar.SetTitle(Resource.String.playlistFrag);
            ((AppCompatActivity)Activity).SupportActionBar.Title = titleName;

            
            targetArea = view.FindViewById<TextView>(Resource.Id.txtTargetArea);
            repsSets = view.FindViewById<TextView>(Resource.Id.txtRepsSets);
            favorite = view.FindViewById<TextView>(Resource.Id.txtFavorite);

            exercise = db.getExercise(titleName);

            targetArea.Text = exercise.targetArea;
            repsSets.Text = exercise.sets + "/" + exercise.reps;
            
            //if (exercise.favorite)
            //{
            //    favorite.Text = "Yes";
            //}
            //else
            //{
            //    favorite.Text = "No";
            //}
            
            return view;
        }        
    }
}