using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using PeakMVP.Helpers;
using PeakMVP.Services.Memory;
using PeakMVP.Services.Navigation;
using PeakMVP.ViewModels.Base;
using System;
using System.Globalization;
using System.Threading;
using Xamarin.Forms;

namespace PeakMVP {
    public partial class App : Application {

        public App() {
            try {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-GB");
            }
            catch (Exception) { }

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(SyncfusionEssential.APP_SECRET_16_3_0_21);

            InitializeComponent();

            InitApp();

#if DEBUG
            TrackMemoryUsage();
#endif
        }

        private void TrackMemoryUsage() {
            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.Android) {
                Xamarin.Forms.Device.StartTimer(TimeSpan.FromSeconds(5), () => {
                    MemoryHelper.DisplayAndroidMemory();

                    return true;
                });
            }
        }

        private void InitApp() {
            ViewModelLocator.RegisterDependencies();
        }

        protected override void OnStart() {
            base.OnStart();

            AppCenter.Start(string.Format("android={0};ios={1}", GlobalSettings.Instance.AzureMobileCenter.AndroidAppSecret, GlobalSettings.Instance.AzureMobileCenter.IOSAppSecret),
                typeof(Analytics), typeof(Crashes));

            InitNavigation();
        }

        private void InitNavigation() {
            INavigationService navigationService = ViewModelLocator.Resolve<INavigationService>();
            navigationService.Initialize(true);
        }

        protected override void OnSleep() {
            Settings.UserProfile = JsonConvert.SerializeObject(GlobalSettings.Instance.UserProfile);
        }

        protected override void OnResume() {
            // Handle when your app resumes
        }
    }
}
