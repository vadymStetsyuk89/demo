using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using PeakMVP.Controls;
using PeakMVP.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ExtendedLabel), typeof(ExtendedLabelRenderer))]
namespace PeakMVP.Droid.Renderers {
    public class ExtendedLabelRenderer : LabelRenderer {

        public ExtendedLabelRenderer(Context context) : base(context) {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Label> e) {
            base.OnElementChanged(e);

            UseLineHeightMultiplier();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e) {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == "LineHeight") {
                UseLineHeightMultiplier();
            }
        }

        private void UseLineHeightMultiplier() {
            try {
                Control.SetLineSpacing(0, ((ExtendedLabel)Element).LineHeight);
            }
            catch (Exception) { }
        }
    }
}