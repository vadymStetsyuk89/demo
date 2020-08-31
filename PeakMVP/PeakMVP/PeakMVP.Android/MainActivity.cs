using Acr.UserDialogs;
using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using CarouselView.FormsPlugin.Android;
using FFImageLoading.Forms.Droid;
using ImageCircle.Forms.Plugin.Droid;
using PeakMVP.Droid.Controlers.Activities;
using PeakMVP.Services.DependencyServices;
using PeakMVP.Views;
using Plugin.CurrentActivity;
using Plugin.Permissions;
using Xamarin.Forms;

namespace PeakMVP.Droid {
    [Activity(
        MainLauncher = true,
        LaunchMode = LaunchMode.SingleInstance,
        ScreenOrientation = ScreenOrientation.Portrait,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : MediaProviderActivity {

        public static MainActivity Self { get; private set; }

        protected override void OnCreate(Bundle bundle) {
            //MessagingCenter.Subscribe<object>(this, CustomNavigationView.ON_CUSTOM_NAVIGATION_VIEW_APPEARING, (param) => {
            //    MessagingCenter.Unsubscribe<object>(this, CustomNavigationView.ON_CUSTOM_NAVIGATION_VIEW_APPEARING);

            //    Window.SetBackgroundDrawableResource(Resource.Drawable.common_window_background_layer_list_drawable);
            //});

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);
            global::Xamarin.Forms.Forms.Init(this, bundle);

            RegisterActivityLifecycleCallbacks(this);

            CarouselViewRenderer.Init();
            UserDialogs.Init(this);
            ImageCircleRenderer.Init();
            CachedImageRenderer.Init(true);

            LoadApplication(new App());

            Self = this;

            //string secureString = Secure.GetString(BaseContext.ContentResolver, Secure.AndroidId);
        }

        private void RegisterActivityLifecycleCallbacks(MainActivity mainActivity) {
            CrossCurrentActivity.Current.Activity = mainActivity;
        }

        /// <summary>
        /// On request permissions result
        /// </summary>
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults) {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}

