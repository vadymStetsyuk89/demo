using Android.Content;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using PeakMVP.Controls;
using PeakMVP.Droid.Renderers;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using static Android.Views.View;

[assembly: ExportRenderer(typeof(ListViewExtended), typeof(ListViewExtendedRenderer))]
namespace PeakMVP.Droid.Renderers {
    public class ListViewExtendedRenderer : ListViewRenderer, IOnTouchListener {

        public ListViewExtendedRenderer(Context context)
            : base(context) { }

        public bool OnTouch(Android.Views.View v, MotionEvent e) {
            try {
                IViewParent parent = null;

                if (v.Parent is Android.Views.View) {
                    parent = v.Parent;
                }

                while (parent != null || parent is Android.Widget.ListView) {
                    parent.RequestDisallowInterceptTouchEvent(true);

                    parent = (parent is Android.Views.View viewParent) ? viewParent.Parent : null;
                }
            }
            catch (Exception exc) {

            }

            return false;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ListView> e) {
            base.OnElementChanged(e);

            if (Control != null) {
                Control.Scroll += OnControlScroll;
                Control.ScrollStateChanged += OnControlScrollStateChanged;
                Control.SetOnTouchListener(this);
                Control.SetSelector(Resource.Layout.no_selector);
            }
        }

        private void OnControlScrollStateChanged(object sender, AbsListView.ScrollStateChangedEventArgs e) {
            switch (e.ScrollState) {
                case ScrollState.Fling:
                    ImageService.Instance.SetPauseWork(true);
                    break;
                case ScrollState.Idle:
                    ImageService.Instance.SetPauseWork(false);
                    break;
                case ScrollState.TouchScroll:
                    break;
                default:
                    break;
            }
        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);

            if (Control != null) {
                Control.Scroll -= OnControlScroll;
            }
        }

        private void OnControlScroll(object sender, AbsListView.ScrollEventArgs e) {
            Android.Views.View view = Control.GetChildAt(0);

            if (view != null) {
                ((ListViewExtended)Element).YScrollOutput = (-view.Top + Control.FirstVisiblePosition * view.Height) / Context.Resources.DisplayMetrics.Density;
            }
        }
    }
}