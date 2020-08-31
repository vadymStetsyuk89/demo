using PeakMVP.Controls;
using PeakMVP.iOS.Renderers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ExtendedEditor), typeof(ExtendedEditorRenderer))]
namespace PeakMVP.iOS.Renderers {
    public class ExtendedEditorRenderer : EditorRenderer {

        //private static readonly UIColor _DEFAULT_TEXT_COLOR = UIColor.Black;
        //private static readonly UIColor _DEFAULT_PLACEHOLDER_COLOR = UIColor.LightGray;

        //public string Placeholder { get; set; }

        ////protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e) {
        ////    base.OnElementPropertyChanged(sender, e);

        ////    if (e.PropertyName == ExtendedEditor.TextProperty.PropertyName) {
        ////         ((ExtendedEditor)Element).Text;
        ////    }
        ////}

        //protected override void OnElementChanged(ElementChangedEventArgs<Editor> e) {
        //    base.OnElementChanged(e);

        //    if (Control != null && Element != null) {
        //        Placeholder = ((ExtendedEditor)Element).Placeholder;

        //        if (string.IsNullOrEmpty(((ExtendedEditor)Element).Text)) {
        //            Control.TextColor = _DEFAULT_PLACEHOLDER_COLOR;
        //            Control.Text = Placeholder;

        //        } else {
        //            Control.TextColor = _DEFAULT_TEXT_COLOR;

        //            Control.Text = ((ExtendedEditor)Element).Text;

        //        }


        //        Control.ShouldBeginEditing += (UITextView textView) => {
        //            if (textView.Text == Placeholder) {
        //                textView.Text = "";
        //                textView.TextColor = _DEFAULT_TEXT_COLOR;
        //            }

        //            return true;
        //        };

        //        Control.ShouldEndEditing += (UITextView textView) => {
        //            if (textView.Text == "") {
        //                textView.Text = Placeholder;
        //                textView.TextColor = _DEFAULT_PLACEHOLDER_COLOR;
        //            }

        //            return true;
        //        };
        //    }
        //}
    }
}
