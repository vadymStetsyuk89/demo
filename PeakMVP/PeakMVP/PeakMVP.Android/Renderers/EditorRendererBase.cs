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
using Xamarin.Forms;
using NativeGraphics = Android.Graphics;
using Xamarin.Forms.Platform.Android;
using Android.Graphics.Drawables;
using PeakMVP.Helpers;
using PeakMVP.Droid.Renderers.Helpers;

namespace PeakMVP.Droid.Renderers {
    public abstract class EditorRendererBase : EditorRenderer {

        public EditorRendererBase(Context context)
            : base(context) {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e) {
            base.OnElementChanged(e);

            if (Control != null && Element != null) {
                RemoveUnderscore();
            }
        }

        private void RemoveUnderscore() {
            if (Control != null && Element != null) {
                Control.Background = new ColorDrawable(BaseSingleton<ValuesResolver>.Instance.ResolveNativeColor(Element.BackgroundColor));
            }
        }
    }
}