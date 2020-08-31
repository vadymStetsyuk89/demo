using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using PeakMVP.Controls;
using PeakMVP.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(PullToRefreshLayout), typeof(PullToRefreshLayoutRenderer))]
namespace PeakMVP.Droid.Renderers {
    public class PullToRefreshLayoutRenderer : SwipeRefreshLayout, IVisualElementRenderer, SwipeRefreshLayout.IOnRefreshListener {

        private bool _init;
        private IVisualElementRenderer _packed;

        public PullToRefreshLayoutRenderer(Context context)
            : base(context) { }

        public static void Init() { }

        public PullToRefreshLayout RefreshView =>
            Element == null ? null : (PullToRefreshLayout)Element;

        private BindableProperty _rendererProperty = null;
        public BindableProperty RendererProperty {
            get {
                if (_rendererProperty != null) {
                    return _rendererProperty;
                }

                Type type = Type.GetType("Xamarin.Forms.Platform.Android.Platform, Xamarin.Forms.Platform.Android");
                FieldInfo prop = type.GetField("RendererProperty", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                object val = prop.GetValue(null);
                _rendererProperty = val as BindableProperty;

                return _rendererProperty;
            }
        }

        private bool _refreshing;
        public override bool Refreshing {
            get {
                return _refreshing;
            }
            set {
                try {
                    _refreshing = value;

                    //
                    //this will break binding :( sad panda we need to wait for next version for this
                    //right now you can't update the binding.. so it is 1 way
                    //
                    if (RefreshView != null && RefreshView.IsRefreshing != _refreshing) {
                        RefreshView.IsRefreshing = _refreshing;
                    }

                    if (base.Refreshing == _refreshing) {
                        return;
                    }

                    base.Refreshing = _refreshing;
                }
                catch (Exception exc) {
                }
            }
        }

        /// <summary>
        /// IVisualElementRenderer implementation
        /// </summary>
        public VisualElement Element { get; private set; }

        /// <summary>
        /// IVisualElementRenderer implementation
        /// </summary>
        public VisualElementTracker Tracker { get; private set; }

        /// <summary>
        /// IVisualElementRenderer implementation
        /// </summary>
        public ViewGroup ViewGroup => this;

        /// <summary>
        /// IVisualElementRenderer implementation
        /// </summary>
        public Android.Views.View View => this;

        /// <summary>
        /// IVisualElementRenderer implementation.
        /// </summary>
        public event EventHandler<VisualElementChangedEventArgs> ElementChanged = delegate { };

        /// <summary>
        /// IVisualElementRenderer implementation
        /// </summary>
        public event EventHandler<PropertyChangedEventArgs> ElementPropertyChanged = delegate { };

        /// <summary>
        /// IVisualElementRenderer implementation
        /// </summary>
        /// <param name="widthConstraint"></param>
        /// <param name="heightConstraint"></param>
        /// <returns></returns>
        public SizeRequest GetDesiredSize(int widthConstraint, int heightConstraint) {
            _packed.View.Measure(widthConstraint, heightConstraint);

            //
            //Measure child here and determine size
            //
            return new SizeRequest(new Size(_packed.View.MeasuredWidth, _packed.View.MeasuredHeight));
        }

        /// <summary>
        /// IVisualElementRenderer implementation
        /// </summary>
        /// <param name="element"></param>
        public void SetElement(VisualElement element) {
            VisualElement oldElement = Element;

            if (oldElement != null) {
                oldElement.PropertyChanged -= HandlePropertyChanged;
            }

            Element = element;

            if (Element != null) {
                UpdateContent();
                Element.PropertyChanged += HandlePropertyChanged;
            }

            if (!_init) {
                _init = true;

                //
                //sizes to match the forms view
                //updates properties, handles visual element properties
                //
                Tracker = new VisualElementTracker(this);
                SetOnRefreshListener(this);
            }

            UpdateColors();
            UpdateIsRefreshing();
            UpdateIsSwipeToRefreshEnabled();

            ElementChanged(this, new VisualElementChangedEventArgs(oldElement, this.Element));
        }

        /// <summary>
        /// IVisualElementRenderer implementation
        /// </summary>
        /// <param name="id"></param>
        public void SetLabelFor(int? id) { }

        /// <summary>
        /// IVisualElementRenderer implementation
        /// </summary>
        public void UpdateLayout() => Tracker?.UpdateLayout();

        /// <summary>
        /// SwipeRefreshLayout.IOnRefreshListener implementation
        /// </summary>
        public void OnRefresh() {
            if (RefreshView?.RefreshCommand?.CanExecute(RefreshView?.RefreshCommandParameter) ?? false) {
                RefreshView.RefreshCommand.Execute(RefreshView?.RefreshCommandParameter);
            }
        }

        public override bool CanChildScrollUp() =>
            CanScrollUp(_packed.View);

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);

            //if (disposing)
            //{
            //    if (Element != null)
            //    {
            //        Element.PropertyChanged -= HandlePropertyChanged;
            //    }
            //    if (packed != null)
            //        RemoveView(packed.ViewGroup);
            //}
            //packed?.Dispose();
            //packed = null;
            //Tracker?.Dispose();
            //Tracker = null;
            
            //if (rendererProperty != null)
            //{
            //    rendererProperty = null;
            //}
            //init = false;
        }

        private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == "Content")
                UpdateContent();
            else if (e.PropertyName == PullToRefreshLayout.IsPullToRefreshEnabledProperty.PropertyName)
                UpdateIsSwipeToRefreshEnabled();
            else if (e.PropertyName == PullToRefreshLayout.IsRefreshingProperty.PropertyName)
                UpdateIsRefreshing();
            else if (e.PropertyName == PullToRefreshLayout.RefreshColorProperty.PropertyName)
                UpdateColors();
            else if (e.PropertyName == PullToRefreshLayout.RefreshBackgroundColorProperty.PropertyName)
                UpdateColors();
        }

        private void UpdateContent() {
            if (RefreshView.Content == null) {
                return;
            }

            if (_packed != null)
                RemoveView(_packed.View);

            _packed = Platform.CreateRendererWithContext(RefreshView.Content, Context);

            try {
                RefreshView.Content.SetValue(RendererProperty, _packed);
            }
            catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine("Unable to sent renderer property, maybe an issue: " + ex);
            }

            AddView(_packed.View, LayoutParams.MatchParent);
        }

        private void UpdateColors() {
            if (RefreshView == null) {
                return;
            }

            if (RefreshView.RefreshColor != Color.Default) {
                SetColorSchemeColors(RefreshView.RefreshColor.ToAndroid());
            }

            if (RefreshView.RefreshBackgroundColor != Color.Default) {
                SetProgressBackgroundColorSchemeColor(RefreshView.RefreshBackgroundColor.ToAndroid());
            }
        }

        private void UpdateIsRefreshing() =>
            Refreshing = RefreshView.IsRefreshing;

        private void UpdateIsSwipeToRefreshEnabled() {
           Enabled = RefreshView.IsPullToRefreshEnabled;
        }

        private bool CanScrollUp(Android.Views.View view) {
            if (!RefreshView.IsPullToRefreshEnabled) {
                return true;
            }
            else {
                ViewGroup viewGroup = view as ViewGroup;
                if (viewGroup == null) {
                    return base.CanChildScrollUp();
                }

                int sdk = (int)global::Android.OS.Build.VERSION.SdkInt;
                if (sdk >= 16) {
                    //
                    //is a scroll container such as listview, scroll view, gridview
                    //
                    if (viewGroup.IsScrollContainer) {
                        return base.CanChildScrollUp();
                    }
                }

                //
                //if you have something custom and you can't scroll up you might need to enable this
                //for instance on a custom recycler view where the code above isn't working!
                //
                for (int i = 0; i < viewGroup.ChildCount; i++) {
                    var child = viewGroup.GetChildAt(i);
                    if (child is Android.Widget.AbsListView) {
                        Android.Widget.AbsListView list = child as Android.Widget.AbsListView;
                        if (list != null) {
                            if (list.FirstVisiblePosition == 0) {
                                Android.Views.View subChild = list.GetChildAt(0);

                                return subChild != null && subChild.Top != 0;
                            }

                            //
                            //if children are in list and we are scrolled a bit... sure you can scroll up
                            //
                            return true;
                        }

                    }
                    else if (child is Android.Widget.ScrollView) {
                        Android.Widget.ScrollView scrollview = child as Android.Widget.ScrollView;
                        return (scrollview.ScrollY <= 0.0);
                    }
                    else if (child is Android.Webkit.WebView) {
                        Android.Webkit.WebView webView = child as Android.Webkit.WebView;
                        return (webView.ScrollY > 0.0);
                    }
                    else if (child is Android.Support.V4.Widget.SwipeRefreshLayout) {
                        return CanScrollUp(child as ViewGroup);
                    }
                    //
                    //else if something else like a recycler view?
                    //
                }

                return false;
            }
        }
    }
}