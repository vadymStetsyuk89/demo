using Android.Content;
using PeakMVP.Controls;
using PeakMVP.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ExtendedDatePicker), typeof(PickMVPDatePickerRenderer))]
namespace PeakMVP.Droid.Renderers {
    public class PickMVPDatePickerRenderer : DatePickerRenderer {

        public PickMVPDatePickerRenderer(Context context)
            : base(context) {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.DatePicker> e) {
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