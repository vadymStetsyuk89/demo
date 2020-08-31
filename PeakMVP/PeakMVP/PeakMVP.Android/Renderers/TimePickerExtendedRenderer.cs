using Android.Content;
using PeakMVP.Controls;
using PeakMVP.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(TimePickerExtended), typeof(TimePickerExtendedRenderer))]
namespace PeakMVP.Droid.Renderers {
    public class TimePickerExtendedRenderer : TimePickerRenderer {

        public TimePickerExtendedRenderer(Context context) 
            : base(context) { }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.TimePicker> e) {
            base.OnElementChanged(e);

            if (Control != null) {
                ///
                /// Removes underline
                /// 
                Control.SetBackgroundColor(Android.Graphics.Color.Transparent);

                ///
                /// Input text layouting adjustment
                /// 
                Android.Support.V4.View.ViewCompat.SetPaddingRelative(Control, 0, 0, 0, 0);
            }
        }
    }
}