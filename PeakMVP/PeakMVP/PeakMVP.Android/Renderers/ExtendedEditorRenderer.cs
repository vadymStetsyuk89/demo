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
using PeakMVP.Droid.Renderers.Helpers;
using PeakMVP.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ExtendedEditor), typeof(ExtendedEditorRenderer))]
namespace PeakMVP.Droid.Renderers {
    public class ExtendedEditorRenderer : EditorRendererBase {

        public ExtendedEditorRenderer(Context context)
            : base(context) {
        }

        //protected override void OnElementChanged(ElementChangedEventArgs<Editor> e) {
        //    base.OnElementChanged(e);

        //    if (e.NewElement != null) {
        //        Control.Hint = ((ExtendedEditor)Element).Placeholder;
        //        ApllyPadding();
        //    }
        //}

        //protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e) {
        //    base.OnElementPropertyChanged(sender, e);

        //    if (e.PropertyName == ExtendedEditor.PlaceholderProperty.PropertyName) {
        //        Control.Hint = ((ExtendedEditor)Element).Placeholder;
        //    }
        //    else if (e.PropertyName == ExtendedEditor.PaddingProperty.PropertyName) {
        //        ApllyPadding();
        //    }
        //}

        //private void ApllyPadding() {
        //    Control.SetPadding(
        //        (int)BaseSingleton<ValuesResolver>.Instance.ResolveSizeValue((float)((ExtendedEditor)Element).Padding.Left),
        //        (int)BaseSingleton<ValuesResolver>.Instance.ResolveSizeValue((float)((ExtendedEditor)Element).Padding.Top),
        //        (int)BaseSingleton<ValuesResolver>.Instance.ResolveSizeValue((float)((ExtendedEditor)Element).Padding.Right),
        //        (int)BaseSingleton<ValuesResolver>.Instance.ResolveSizeValue((float)((ExtendedEditor)Element).Padding.Bottom));
        //}
    }
}