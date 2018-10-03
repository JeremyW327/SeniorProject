using Android.OS;
using Android.Support.V4.App;
using Android.Views;

using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using System;
using Android.Support.V7.App;
using Android.Widget;
using System.Collections.Generic;
using System.Timers;

namespace ProjectPE
{
    
    public class timerFragment : Fragment
    {
        //Buttons, EditTexts, and TextView for the layout of the timer
        Button startButton;
        Button resetButton;       

        
        EditText edtMinutes;
        EditText edtSeconds;

        TextView txtTimer;

        //declare Timer
        Timer timer;

        //declare the numbers require for the timer to run
        int timeLeft;
        int timePassed;
        int minutes;
        int seconds;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static timerFragment NewInstance()
        {
            var frag1 = new timerFragment { Arguments = new Bundle() };
            return frag1;
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.TimerFragment, null);
            ((AppCompatActivity)Activity).SupportActionBar.SetTitle(Resource.String.frag5Title);
            var ignored = base.OnCreateView(inflater, container, savedInstanceState);

            //set the views for buttons, edittexts and textview
            startButton = view.FindViewById<Button>(Resource.Id.btnStart);
            resetButton = view.FindViewById<Button>(Resource.Id.btnReset);
            

           
            edtMinutes = view.FindViewById<EditText>(Resource.Id.txtMinutes);
            edtSeconds = view.FindViewById<EditText>(Resource.Id.txtSeconds);

            txtTimer = view.FindViewById<TextView>(Resource.Id.txtTimer);

            //Button Start click event
            startButton.Click += delegate
            {
                if (edtMinutes.Text == null && edtSeconds.Text == null)
                {
                    //Do nothing
                }
                else if (startButton.Text == "Pause")
                {
                    timer.Stop();
                    startButton.Text = "Start";
                }
                else
                {
                    //set button text to pause
                    startButton.Text = "Pause";

                    //check for null values and change them to 0 so the math works.
                    if (string.IsNullOrEmpty(edtMinutes.Text))
                        edtMinutes.Text = "0";
                    if (string.IsNullOrEmpty(edtSeconds.Text))
                        edtSeconds.Text = "0";                    
                    minutes = Convert.ToInt32(edtMinutes.Text);
                    seconds = Convert.ToInt32(edtSeconds.Text);
                    timeLeft = ((minutes * 60) + seconds) - timePassed;

                    timer = new Timer();
                    timer.Elapsed += Timer_Elapsed;
                    timer.Interval = 1000; //set interval to 1 second
                    timer.Start();
                }

            };

            resetButton.Click += delegate
            {
                edtMinutes.Text = "";
                edtSeconds.Text = "";
                txtTimer.Text = "00:00";
                startButton.Text = "Start";
                minutes = 0;
                seconds = 0;
                timePassed = 0;

                timer.Stop();
                timer.Dispose();
                timer = null;
            };          

            return view;
        }       

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (timeLeft > 0)
            {
                timeLeft = timeLeft - 1;
                timePassed = timePassed + 1;

                var timeSpan = TimeSpan.FromSeconds(timeLeft);
                this.Activity.RunOnUiThread(() => { txtTimer.Text = timeSpan.ToString(@"mm\:ss"); });
                
                //txtTimer.Text = timeSpan.ToString(@"mm\:ss");
            }
            else
            {
                timer.Stop();
            }
            
        }
    }

    
    
}
