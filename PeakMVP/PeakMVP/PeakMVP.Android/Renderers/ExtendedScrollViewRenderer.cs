using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using static Android.Views.View;

[assembly: ExportRenderer(typeof(ExtendedScrollView), typeof(ExtendedScrollViewRenderer))]
namespace PeakMVP.Droid.Renderers {
    public class ExtendedScrollViewRenderer : ScrollViewRenderer {

        private Android.Views.View _childContentView;

        public ExtendedScrollViewRenderer(Context context)
            : base(context) { }

        public bool InterceptEvent { get; private set; } = true;

        protected override void OnElementChanged(VisualElementChangedEventArgs e) {
            base.OnElementChanged(e);

            this.SetOnTouchListener(new ExtendedScrollTouchListener(this));
        }

        //protected override void OnScrollChanged(int x, int y, int oldX, int oldY) {
        //    base.OnScrollChanged(x, y, oldX, oldY);

        //    if (_childContentView == null) {
        //        _childContentView = (Android.Views.View)GetChildAt(ChildCount - 1);
        //    }

        //    if (_childContentView == null) {
        //        InterceptEvent = false;
        //    }
        //    else {
        //        int bottomLimit = (_childContentView.Bottom - (Height + y));

        //        if (bottomLimit <= 0 || y <= 0) {
        //            InterceptEvent = false;
        //        }
        //        else {
        //            InterceptEvent = true;
        //        }
        //    }
        //}
    }

    public class ExtendedScrollTouchListener : Java.Lang.Object, IOnTouchListener {

        private ExtendedScrollViewRenderer _extendedScrollViewRenderer;

        public ExtendedScrollTouchListener(ExtendedScrollViewRenderer extendedScrollViewRenderer) {
            _extendedScrollViewRenderer = extendedScrollViewRenderer;
        }

        public bool OnTouch(Android.Views.View v, MotionEvent e) {
            try {
                IViewParent parent = null;

                if (v.Parent is Android.Views.View) {
                    parent = v.Parent;
                }

                while (parent != null || parent is Android.Widget.ListView || parent is Android.Widget.ScrollView) {
                    parent.RequestDisallowInterceptTouchEvent(true);

                    parent = (parent is Android.Views.View viewParent) ? viewParent.Parent : null;
                }
            }
            catch (Exception exc) {
                Debugger.Break();
            }

            return false;
        }

        //public bool OnTouch(Android.Views.View v, MotionEvent e) {
        //    v.Parent.RequestDisallowInterceptTouchEvent(_extendedScrollViewRenderer.InterceptEvent);

        //    return false;
        //}
    }
}