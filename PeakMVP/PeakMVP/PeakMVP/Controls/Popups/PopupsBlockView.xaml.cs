using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace PeakMVP.Controls.Popups {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PopupsBlockView : ContentView {

        private static readonly string _AFFILIATE_POPUP_ERROR_MESSAGE = "Error occurs while attaching new popup in PopupsBlockView.AffiliateNewPopup(IPopupContext popupContext). Details in inner exception.";
        private static readonly string _DETACH_POPUP_ERROR_MESSAGE = "Error occurs while detaching popup in DetachPopup(IPopupContext popupContext). Details in inner exception.";
        private static readonly string _IS_SINGLE_POPUP_VISIBLE_PBINDING_PATH = "IsPopupVisible";

        private static Color _DEFAULT_BACKING_COLOR = Color.Black;
        private static double _DEFAULT_BACKING_OPACITY = .5;

        public static readonly BindableProperty PopupsProperty = BindableProperty.Create(
            nameof(Popups),
            typeof(IEnumerable<IPopupContext>),
            typeof(PopupsBlockView),
            defaultValue: null,
            propertyChanged: (BindableObject bindable, object oldValue, object newValue) => {
                if (bindable is PopupsBlockView popupsBlockView) {
                    if (newValue is INotifyCollectionChanged newNotifyCollection) {
                        newNotifyCollection.CollectionChanged += popupsBlockView.OnPopupsCollectionCollectionChanged;
                    }

                    if (oldValue is INotifyCollectionChanged oldNotifyCollection) {
                        oldNotifyCollection.CollectionChanged -= popupsBlockView.OnPopupsCollectionCollectionChanged;
                    }

                    if (newValue is IEnumerable<IPopupContext> newCollection) {
                        newCollection.ForEach(iPC => popupsBlockView.AffiliateNewPopup(iPC));
                    }

                    if (oldValue is IEnumerable<IPopupContext> oldCollection) {
                        oldCollection.ForEach(iPC => popupsBlockView.DetachPopup(iPC));
                    }
                }
            });

        public PopupsBlockView() {
            InitializeComponent();

            BackingOpacity = PopupsBlockView._DEFAULT_BACKING_OPACITY;
            BackingColor = PopupsBlockView._DEFAULT_BACKING_COLOR;
        }

        public IEnumerable<IPopupContext> Popups {
            get => (IEnumerable<IPopupContext>)GetValue(PopupsProperty);
            set => SetValue(PopupsProperty, value);
        }

        public BoxView Backing {
            get => _backing_BoxView;
        }

        private Color _backingColor;
        public Color BackingColor {
            get => _backingColor;
            set {
                _backingColor = value;
                _backing_BoxView.BackgroundColor = value;
            }
        }

        private double _backingOpacity;
        public double BackingOpacity {
            get => _backingOpacity;
            set {
                value = (value > 1) ? 1 : value;
                value = (value < 0) ? 0 : value;

                _backingOpacity = value;
                _backing_BoxView.Opacity = value;
            }
        }

        public void CloseAllPopups() {
            _mainContentSpot_Grid.Children.OfType<SinglePopup>().ForEach((sP) => sP.IsViewable = false);
        }

        private void AffiliateNewPopup(IPopupContext popupContext) {
            try {
                SinglePopup singlePopupView = (SinglePopup)new DataTemplate(popupContext.RelativeViewType).CreateContent();
                singlePopupView.BindingContext = popupContext;
                singlePopupView.SetBinding(SinglePopup.IsViewableProperty, new Binding(_IS_SINGLE_POPUP_VISIBLE_PBINDING_PATH, mode: BindingMode.TwoWay));
                singlePopupView.Viewable += OnSinglePopupViewable;

                _mainContentSpot_Grid.Children.Add(singlePopupView);

                OnSinglePopupViewable(singlePopupView, null);
            }
            catch (Exception exc) {
                Debugger.Break();

                throw new InvalidOperationException(_AFFILIATE_POPUP_ERROR_MESSAGE, exc);
            }
        }

        private void DetachPopup(IPopupContext popupContext) {
            try {
                SinglePopup popupViewToRemove = (SinglePopup)_mainContentSpot_Grid.Children.First(c => c.BindingContext == popupContext);
                popupViewToRemove.Viewable -= OnSinglePopupViewable;
                _mainContentSpot_Grid.Children.Remove(popupViewToRemove);
            }
            catch (Exception exc) {
                Debugger.Break();

                throw new InvalidOperationException(_DETACH_POPUP_ERROR_MESSAGE, exc);
            }
        }

        private void OnSinglePopupViewable(object sender, EventArgs e) {
            SinglePopup singlePopup = (SinglePopup)sender;

            double yOffset = (singlePopup.IsViewable) ? 0 : short.MaxValue;

            singlePopup.TranslationY = yOffset;
        }

        private void OnPopupsCollectionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            if (e.Action == NotifyCollectionChangedAction.Add) {
                foreach (object item in e.NewItems) {
                    AffiliateNewPopup(item as IPopupContext);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove) {
                foreach (object item in e.OldItems) {
                    DetachPopup(item as IPopupContext);
                }
            }
            else {
                Debugger.Break();
                throw new NotImplementedException("PopupsBlockView.OnPopupsCollectionCollectionChanged");
            }
        }
    }
}