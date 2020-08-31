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

[assembly: ExportRenderer(typeof(TimePickerExtended), typeof(TimePickerExtendedRenderer))]
namespace PeakMVP.iOS.Renderers {
    public class TimePickerExtendedRenderer: TimePickerRenderer {
        protected override void OnElementChanged(ElementChangedEventArgs<TimePicker> e) {
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