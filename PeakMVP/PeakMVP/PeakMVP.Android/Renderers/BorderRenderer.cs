using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms.Platform.Android;

namespace PeakMVP.Droid.Renderers {
    public class BorderRenderer : IDisposable {

        private GradientDrawable _background;

        public void Dispose() {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                if (_background != null) {
                    _background.Dispose();
                    _background = null;
                }
            }
        }

        public Drawable GetBorderBackground(Xamarin.Forms.Color borderColor, Xamarin.Forms.Color backgroundColor, float borderWidth, float borderRadius) {
            if (_background != null) {
                _background.Dispose();
                _background = null;
            }
            borderWidth = borderWidth > 0 ? borderWidth : 0;
            borderRadius = borderRadius > 0 ? borderRadius : 0;
            borderColor = borderColor != Xamarin.Forms.Color.Default ? borderColor : Xamarin.Forms.Color.Transparent;
            backgroundColor = backgroundColor != Xamarin.Forms.Color.Default ? backgroundColor : Xamarin.Forms.Color.Transparent;

            var strokeWidth = Application.Context.ToPixels(borderWidth);
            var radius = Application.Context.ToPixels(borderRadius);
            _background = new GradientDrawable();
            _background.SetColor(backgroundColor.ToAndroid());
            if (radius > 0)
                _background.SetCornerRadius(radius);
            if (borderColor != Xamarin.Forms.Color.Transparent && strokeWidth > 0) {
                _background.SetStroke((int)strokeWidth, borderColor.ToAndroid());
            }
            return _background;
        }
    }
}