using PeakMVP.Controls;
using PeakMVP.iOS.Renderers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(PullToRefreshLayout), typeof(PullToRefreshLayoutRenderer))]
namespace PeakMVP.iOS.Renderers {
    public class PullToRefreshLayoutRenderer : ViewRenderer<PullToRefreshLayout, UIView> {

        private bool _set;
        private nfloat _origininalY;
        private UIRefreshControl _refreshControl;

        BindableProperty _rendererProperty;
        BindableProperty RendererProperty {
            get {
                if (_rendererProperty != null)
                    return _rendererProperty;

                var type = Type.GetType("Xamarin.Forms.Platform.iOS.Platform, Xamarin.Forms.Platform.iOS");
                var prop = type.GetField("RendererProperty");
                var val = prop.GetValue(null);
                _rendererProperty = val as BindableProperty;

                return _rendererProperty;
            }
        }

        bool _isRefreshing;
        public bool IsRefreshing {
            get { return _isRefreshing; }
            set {
                bool changed = IsRefreshing != value;

                _isRefreshing = value;
                if (_isRefreshing)
                    _refreshControl.BeginRefreshing();
                else
                    _refreshControl.EndRefreshing();

                if (changed)
                    TryOffsetRefresh(this, IsRefreshing);
            }
        }

        public PullToRefreshLayout RefreshView {
            get { return Element; }
        }

        public static void Init() { }

        protected override void OnElementChanged(ElementChangedEventArgs<PullToRefreshLayout> e) {
            base.OnElementChanged(e);

            if (e.OldElement != null || Element == null) return;

            _refreshControl = new UIRefreshControl();

            _refreshControl.ValueChanged += OnRefresh;

            try {
                TryInsertRefresh(this);
            } catch (Exception ex) {
                Debug.WriteLine("View is not supported in PullToRefreshLayout: " + ex);
            }

            UpdateColors();
            UpdateIsRefreshing();
            UpdateIsSwipeToRefreshEnabled();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e) {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == PullToRefreshLayout.IsPullToRefreshEnabledProperty.PropertyName)
                UpdateIsSwipeToRefreshEnabled();
            else if (e.PropertyName == PullToRefreshLayout.IsRefreshingProperty.PropertyName)
                UpdateIsRefreshing();
            else if (e.PropertyName == PullToRefreshLayout.RefreshColorProperty.PropertyName)
                UpdateColors();
            else if (e.PropertyName == PullToRefreshLayout.RefreshBackgroundColorProperty.PropertyName)
                UpdateColors();
        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
            if (_refreshControl != null) {
                _refreshControl.ValueChanged -= OnRefresh;
            }
        }

        private bool TryOffsetRefresh(UIView view, bool refreshing, int index = 0) {
            if (RefreshView.IsPullToRefreshEnabled) {
                if (view is UITableView) {
                    var uiTableView = view as UITableView;
                    if (!_set) {
                        _origininalY = uiTableView.ContentOffset.Y;
                        _set = true;
                    }

                    if (refreshing)
                        uiTableView.SetContentOffset(new CoreGraphics.CGPoint(0, _origininalY - _refreshControl.Frame.Size.Height), true);
                    else
                        uiTableView.SetContentOffset(new CoreGraphics.CGPoint(0, _origininalY), true);
                    return true;
                }

                if (view is UICollectionView) {

                    var uiCollectionView = view as UICollectionView;
                    if (!_set) {
                        _origininalY = uiCollectionView.ContentOffset.Y;
                        _set = true;
                    }
                    if (refreshing)
                        uiCollectionView.SetContentOffset(new CoreGraphics.CGPoint(0, _origininalY - _refreshControl.Frame.Size.Height), true);
                    else
                        uiCollectionView.SetContentOffset(new CoreGraphics.CGPoint(0, _origininalY), true);
                    return true;
                }


                if (view is UIWebView) {
                    //
                    //can't do anything
                    //
                    return true;
                }


                if (view is UIScrollView) {
                    var uiScrollView = view as UIScrollView;

                    if (!_set) {
                        _origininalY = uiScrollView.ContentOffset.Y;
                        _set = true;
                    }

                    if (refreshing)
                        uiScrollView.SetContentOffset(new CoreGraphics.CGPoint(0, _origininalY - _refreshControl.Frame.Size.Height), true);
                    else
                        uiScrollView.SetContentOffset(new CoreGraphics.CGPoint(0, _origininalY), true);
                    return true;
                }

                if (view.Subviews == null)
                    return false;

                //for (int i = 0; i < view.Subviews.Length; i++) {
                //    var control = view.Subviews[i];
                //    if (TryOffsetRefresh(control, refreshing, i))
                //        return true;
                //}

                return false;
            } else {
                return false;
            }
        }

        private bool TryInsertRefresh(UIView view, int index = 0) {

            if (RefreshView.IsPullToRefreshEnabled) {
                if (view is UITableView) {
                    var uiTableView = view as UITableView;
                    uiTableView = view as UITableView;
                    view.InsertSubview(_refreshControl, index);
                    return true;
                }

                if (view is UICollectionView) {
                    var uiCollectionView = view as UICollectionView;
                    uiCollectionView = view as UICollectionView;
                    view.InsertSubview(_refreshControl, index);
                    return true;
                }

                if (view is UIWebView) {
                    var uiWebView = view as UIWebView;
                    uiWebView.ScrollView.InsertSubview(_refreshControl, index);
                    return true;
                }

                if (view is UIScrollView) {
                    var uiScrollView = view as UIScrollView;
                    view.InsertSubview(_refreshControl, index);
                    uiScrollView.AlwaysBounceVertical = true;
                    return true;
                }

                if (view.Subviews == null) {
                    return false;
                }

                for (int i = 0; i < view.Subviews.Length; i++) {
                    var control = view.Subviews[i];
                    if (TryInsertRefresh(control, i))
                        return true;
                }

                return false;
            } else {
                return false;
            }
        }

        private void UpdateColors() {
            if (RefreshView == null) {
                return;
            }
            if (RefreshView.RefreshColor != Color.Default) {
                _refreshControl.TintColor = RefreshView.RefreshColor.ToUIColor();
            }
            if (RefreshView.RefreshBackgroundColor != Color.Default) {
                _refreshControl.BackgroundColor = RefreshView.RefreshBackgroundColor.ToUIColor();
            }
        }

        private void UpdateIsRefreshing() {
            IsRefreshing = RefreshView.IsRefreshing;
        }

        private void UpdateIsSwipeToRefreshEnabled() {
            _refreshControl.Enabled = RefreshView.IsPullToRefreshEnabled;
        }

        private void OnRefresh(object sender, EventArgs e) {
            if (RefreshView?.RefreshCommand?.CanExecute(RefreshView?.RefreshCommandParameter) ?? false) {
                RefreshView.RefreshCommand.Execute(RefreshView?.RefreshCommandParameter);
            }
        }
    }
}
