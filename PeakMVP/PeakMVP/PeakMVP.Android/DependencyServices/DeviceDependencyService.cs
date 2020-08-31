using Android.App;
using Microsoft.AppCenter.Crashes;
using PeakMVP.Droid.DependencyServices;
using PeakMVP.Services.DependencyServices;
using System;
using System.Diagnostics;
using Xamarin.Forms;

[assembly: Dependency(typeof(DeviceDependencyService))]
namespace PeakMVP.Droid.DependencyServices {

    public class DeviceDependencyService : IDeviceDependencyService {
        public void SetCommonTheme() {
            try {
                MainActivity.Self.Window.SetBackgroundDrawableResource(Resource.Drawable.common_window_background_layer_list_drawable);
            }
            catch (Exception exc) {
                Crashes.TrackError(exc);
                Debugger.Break();
            }
        }

        public void SetDistinguishingTheme() {
            try {
                MainActivity.Self.Window.SetBackgroundDrawableResource(Resource.Drawable.splashscreen_layer_list_drawable);
            }
            catch (Exception exc) {
                Crashes.TrackError(exc);
                Debugger.Break();
            }
        }
    }
}