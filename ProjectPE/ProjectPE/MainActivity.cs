using Android.App;
using Android.OS;
using ProjectPE.Fragments;

using Android.Support.Design.Widget;
using Android.Support.V7.App;
using System.IO;
using SQLite;
using Android.Widget;
using Android.Views;
using Android.Support.V7.Widget;
using ProjectPE.Resources.DataHelper;
using Android.Content.Res;

namespace ProjectPE
{
    [Activity(Label = "@string/app_name", MainLauncher = true, LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Icon = "@drawable/icon", WindowSoftInputMode = SoftInput.AdjustPan, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity
    {
        DataBase db = new DataBase();

        BottomNavigationView bottomNavigation;
        protected override void OnCreate(Bundle bundle)
        { 
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.main);

            //create database            
            db.createDatabase();

            //setup bottom nav bar
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            if (toolbar != null)
            {
                SetSupportActionBar(toolbar);                
                SupportActionBar.SetDisplayHomeAsUpEnabled(false);
                SupportActionBar.SetHomeButtonEnabled(false);              
            }

            bottomNavigation = FindViewById<BottomNavigationView>(Resource.Id.bottom_navigation);
            BottomNavigationViewUtils.SetShiftMode(bottomNavigation, false, false);

            bottomNavigation.NavigationItemSelected += BottomNavigation_NavigationItemSelected;

            //load the home fragment
            LoadFragment(Resource.Id.menu_home);
        }

        private void BottomNavigation_NavigationItemSelected(object sender, BottomNavigationView.NavigationItemSelectedEventArgs e)
        {
            LoadFragment(e.Item.ItemId);
        }

        void LoadFragment(int id)
        {
            Android.Support.V4.App.Fragment fragment = null;
           
            switch (id)
            {
                case Resource.Id.menu_home:
                    fragment = homeFragment.NewInstance();
                   
                    break;
                case Resource.Id.menu_playlists:
                    fragment = playlistsFragment.NewInstance();
                    
                    break;
                case Resource.Id.menu_browse:
                    fragment = browseFragment.NewInstance();
                    
                    break;
                case Resource.Id.menu_search:
                    fragment = searchFragment.NewInstance();
                    
                    break;
                case Resource.Id.menu_timer:
                    fragment = timerFragment.NewInstance();
                    
                    break;                
            }
            if (fragment == null)
                return;

            SupportFragmentManager.BeginTransaction()
               .Replace(Resource.Id.content_frame, fragment)
               .AddToBackStack(null)
               .Commit();
        }

        public override void OnBackPressed()
        {
            if (SupportFragmentManager.BackStackEntryCount > 0)
            {
                SupportFragmentManager.PopBackStack();                                            
            }
            else
            {
                base.OnBackPressed();
                
            }           
        }       

    }
}

