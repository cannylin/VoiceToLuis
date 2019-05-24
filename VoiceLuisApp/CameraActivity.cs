using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace VoiceLuisApp
{
	[Activity (Label = "Camera main")]//, MainLauncher = true, Icon = "@drawable/icon"
    public class CameraActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
		   	ActionBar.Hide ();
			SetContentView (Resource.Layout.activity_camera);

          BackToMainTime(4000);

            if (bundle == null) {
				FragmentManager.BeginTransaction ().Replace (Resource.Id.container, CameraBasic.NewInstance ()).Commit ();
			}


        }


        public void EndToBack_Job()
        {
            Intent intent = new Intent(this, typeof(VoiceToLuisActivity));
            StartActivity(intent);
        }


        public void BackToMainTime(int DelayValue)
        {
            System.Timers.Timer Timer1 = new System.Timers.Timer();
            Timer1.Interval = DelayValue;
            Timer1.Enabled = true;
            Timer1.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) =>
            {

                Intent intent = new Intent(this, typeof(VoiceToLuisActivity));

                intent.PutExtra("para3",3);
                StartActivity(intent);

                Timer1.Stop();
            };
            Timer1.Start();
        }












    }
}


