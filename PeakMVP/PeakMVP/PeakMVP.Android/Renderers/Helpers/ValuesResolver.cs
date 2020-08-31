using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Xamarin.Forms.Platform.Android;

namespace PeakMVP.Droid.Renderers.Helpers {
    public class ValuesResolver {

        public float ResolveSizeValue(float dependentValue) {
            //return dependentValue * Application.Context.Resources.DisplayMetrics.Density;
            return Application.Context.ToPixels(dependentValue);
        }

        public Color ResolveNativeColor(Xamarin.Forms.Color color) {
            byte red = (byte)(color.R * 255);
            byte green = (byte)(color.G * 255);
            byte blue = (byte)(color.B * 255);
            byte alpha = (byte)(color.A * 255);

            return new Color(red, green, blue, alpha);
        }
    }
}