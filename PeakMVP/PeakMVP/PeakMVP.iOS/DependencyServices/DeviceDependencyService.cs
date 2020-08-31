using Microsoft.AppCenter.Crashes;
using PeakMVP.iOS.DependencyServices;
using PeakMVP.Services.DependencyServices;
using System;
using System.Diagnostics;
using Xamarin.Forms;

[assembly: Dependency(typeof(DeviceDependencyService))]
namespace PeakMVP.iOS.DependencyServices {
    public class DeviceDependencyService : IDeviceDependencyService {
        public void SetCommonTheme() {
            try {
                //AppDelegate appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
                //UIWindow uIWindow = appDelegate.Window;

                //UIImageView uIImageView = new UIImageView(uIWindow.Frame);
                //uIImageView.Image = new UIImage("Default.png");

                //uIWindow.AddSubview(uIImageView);
            }
            catch (Exception exc) {
                Crashes.TrackError(exc);
                Debugger.Break();
            }
        }
        
        public void SetDistinguishingTheme() {
            try {
                //AppDelegate appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
                //UIWindow uIWindow = appDelegate.Window;

                //UIImageView uIImageView = new UIImageView(uIWindow.Frame);
                //uIImageView.Image = new UIImage("Icon-76.png");

                //uIWindow.AddSubview(uIImageView);
            }
            catch (Exception exc) {
                Crashes.TrackError(exc);
                Debugger.Break();
            }
        }
    }
}