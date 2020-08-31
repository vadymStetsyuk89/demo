using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace PeakMVP.Droid.Controlers.Activities {

    [Activity(
        MainLauncher = false,
        LaunchMode = LaunchMode.SingleInstance,
        ScreenOrientation = ScreenOrientation.Portrait,
        Theme = "@style/SplashScreenTheme")]
    public class SplashScreenActivity : Android.Support.V7.App.AppCompatActivity {

        protected override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);

            //
            // Is not necessary. Status bar visibility changes from theme
            //
            //Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);

            NavigateToMainActivity();
        }

        private void NavigateToMainActivity() {
            Intent intent = new Intent(this, typeof(MainActivity));
            ActivityOptions activityOptions = ActivityOptions.MakeCustomAnimation(this, Resource.Animation.abc_fade_in, Resource.Animation.abc_fade_out);

            StartActivity(intent, activityOptions.ToBundle());
            Finish();
        }
    }
}