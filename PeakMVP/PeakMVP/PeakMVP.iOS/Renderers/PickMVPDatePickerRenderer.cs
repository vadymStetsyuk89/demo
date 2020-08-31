using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using PeakMVP.Controls;
using PeakMVP.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ExtendedDatePicker), typeof(PickMVPDatePickerRenderer))]
namespace PeakMVP.iOS.Renderers {
    public class PickMVPDatePickerRenderer : DatePickerRenderer {

        protected override void OnElementChanged(ElementChangedEventArgs<DatePicker> e) {
            base.OnElementChanged(e);

            if (Control != null) {
                DisableNativeBorder();
            }

            if (e.OldElement != null) {
                // Unsubscribe from event handlers and cleanup any resources
            }

            if (e.NewElement != null) {
                // Configure the control and subscribe to event handlers
            }
        }

        private void DisableNativeBorder() {
            Control.BorderStyle = UIKit.UITextBorderStyle.None;
        }
    }
}