using PeakMVP.Controls;
using PeakMVP.Controls.ActionBars.Base;
using PeakMVP.Controls.Popups;
using PeakMVP.Models.AppNavigation;
using PeakMVP.Views.CompoundedViews.MainContent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace PeakMVP.Views.Base {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public abstract partial class ContentPageBase : ContentPage {

        private static readonly string _BUSY_BINDING_PATH = "IsBusy";
        private static readonly string _IS_POPUP_VISIBLE_BINDING_PATH = "IsPopupsVisible";
        private static readonly string _APP_BACKGROUND_IMAGE_BINDING_PATH = "AppBackgroundImage";
        private static readonly string _POPUPS_BINDING_PATH = "Popups";
        private static readonly string _NAVIGATION_MODES_BINDING_PATH = "ActionBarViewModel.Modes";
        private static readonly string _SELECTED_NAVIGATION_MODE_BINDING_PATH = "ActionBarViewModel.SelectedMode";
        private static readonly string _SELECTED_BOTTOM_ITEM_INDEX_BINDING_PATH = "SelectedNavigationMode.SelectedBarItemIndex";

        private static readonly string _IS_PULL_TO_REFRESH_ENABLED_BINDING_PATH = "IsPullToRefreshEnabled";
        private static readonly string _IS_REFRESHING_BINDING_PATH = "IsRefreshing";
        private static readonly string _REFRESH_COMMAND_BINDING_PATH = "RefreshCommand";

        public static readonly BindableProperty MainContentProperty =
            BindableProperty.Create(nameof(MainContent),
                typeof(View),
                typeof(ContentPageBase),
                defaultValue: null,
                propertyChanged: (BindableObject bindable, object oldValue, object newValue) => {
                    if (bindable is ContentPageBase declarer) {
                        declarer._contentBox_Grid.Children.Add(newValue as View);
                    }
                });

        public static readonly BindableProperty ActionBarProperty =
            BindableProperty.Create(nameof(ActionBar),
                typeof(ActionBarBase),
                typeof(ContentPageBase),
                defaultValue: null,
                propertyChanged: (BindableObject bindable, object oldValue, object newValue) => {
                    if (bindable is ContentPageBase declarer) {
                        declarer._actionBarSpot_ContentView.Content = newValue as ActionBarBase;
                    }
                });

        public static readonly BindableProperty IsBusyAwaitingProperty =
            BindableProperty.Create(nameof(IsBusyAwaiting),
                typeof(bool),
                typeof(ContentPageBase),
                defaultValue: false,
                propertyChanged: (BindableObject bindable, object oldValue, object newValue) => {
                    if (bindable is ContentPageBase declarer) {
                        declarer._busyIndicator_Indicator.IsVisible = declarer.IsBusyAwaiting;

                        if (declarer.IsBusyAwaiting) {
                            Grid.SetRow(declarer._busyIndicator_Indicator, 0);
                        }
                        else {
                            Grid.SetRow(declarer._busyIndicator_Indicator, 1);
                        }
                    }
                });

        public static readonly BindableProperty IsPopupVisibleProperty =
            BindableProperty.Create(nameof(IsPopupVisible),
                typeof(bool),
                typeof(ContentPageBase),
                defaultValue: false,
                propertyChanged: async (BindableObject bindable, object oldValue, object newValue) => {
                    if (bindable is ContentPageBase declarer) {
                        if (declarer.IsPopupVisible) {
                            declarer._popupSpot_ContentView.Opacity = 0;
                            Grid.SetRow(declarer._popupSpot_ContentView, 0);
                            await declarer._popupSpot_ContentView.FadeTo(1);
                        }
                        else {
                            declarer._popupSpot_ContentView.Opacity = 1;
                            await declarer._popupSpot_ContentView.FadeTo(0);
                            Grid.SetRow(declarer._popupSpot_ContentView, 1);

                            declarer._popupsKeeper_PopupsBlockView.CloseAllPopups();
                        }
                    }
                });

        public static readonly BindableProperty NavigationModesProperty =
            BindableProperty.Create(nameof(NavigationModes),
                typeof(IReadOnlyCollection<NavigationModeBase>),
                typeof(ContentPageBase),
                defaultValue: null,
                propertyChanged: (BindableObject bindable, object oldValue, object newValue) => {
                    if (bindable is ContentPageBase declarer) {
                        declarer._bottomBarSpot_Grid.Children.Clear();
                        declarer._bottomBarSpot_Grid.ColumnDefinitions.Clear();
                        declarer._contentBox_Grid.Children.Clear();

                        declarer._bottomBarModeViewPairs.Clear();

                        if (declarer.NavigationModes != null) {
                            foreach (NavigationModeBase modeItem in declarer.NavigationModes) {
                                Grid bottomItemsRow = new Grid();
                                bottomItemsRow.ColumnSpacing = 0;

                                for (int i = 0; i < modeItem.BarItems.Count; i++) {
                                    SingleBottomItem singleVisualBottomItem = new SingleBottomItem();
                                    singleVisualBottomItem.TabIndex = i;
                                    singleVisualBottomItem.BindingContext = modeItem.BarItems.ElementAt(i);
                                    singleVisualBottomItem.GestureRecognizers.Add(declarer._bottomItemTapGestureRecognizer);
                                    Grid.SetColumn(singleVisualBottomItem, i);

                                    bottomItemsRow.ColumnDefinitions.Add(new ColumnDefinition() {
                                        Width = new GridLength(1, GridUnitType.Star)
                                    });
                                    bottomItemsRow.Children.Add(singleVisualBottomItem);

                                    //declarer._contentBox_Grid.Children.Add(singleVisualBottomItem.AppropriateItemContentView);
                                    declarer._bottomBarSpot_Grid.Children.Add(bottomItemsRow);
                                }

                                declarer._bottomBarModeViewPairs.Add(modeItem, bottomItemsRow);
                            }
                        }
                    }
                });

        public static readonly BindableProperty SelectedNavigationModeProperty =
            BindableProperty.Create(nameof(SelectedNavigationMode),
                typeof(NavigationModeBase),
                typeof(ContentPageBase),
                defaultValue: null,
                propertyChanged: (BindableObject bindable, object oldValue, object newValue) => {
                    if (bindable is ContentPageBase declarer) {
                        Grid requiredBottomItemsGrid = declarer._bottomBarModeViewPairs[declarer.SelectedNavigationMode];

                        declarer._bottomBarSpot_Grid.Children.ForEach(c => c.TranslationX = (c == requiredBottomItemsGrid) ? 0 : short.MaxValue);
                        declarer.SelectedBottomItemIndex = 0;
                        if (declarer.SelectedBottomItemIndex != 0) {
                            declarer.SelectedBottomItemIndex = 0;
                        }
                        else {
                            declarer.OnSelectedBottomItemIndex();
                        }

                        if (oldValue != null) {
                            ((NavigationModeBase)oldValue).SelectedBarItemIndex = 0;
                        }
                    }
                });

        public static readonly BindableProperty SelectedBottomItemIndexProperty =
            BindableProperty.Create(nameof(SelectedBottomItemIndex),
                typeof(int),
                typeof(ContentPageBase),
                defaultValue: -1,
                propertyChanged: (BindableObject bindable, object oldValue, object newValue) => (bindable as ContentPageBase)?.OnSelectedBottomItemIndex());

        public static readonly BindableProperty IsPullToRefreshEnabledProperty =
            BindableProperty.Create(nameof(IsPullToRefreshEnabled),
                typeof(bool),
                typeof(ContentPageBase),
                defaultValue: default(bool));

        public static readonly BindableProperty IsRefreshingProperty =
            BindableProperty.Create(nameof(IsRefreshing),
                typeof(bool),
                typeof(ContentPageBase),
                defaultValue: default(bool));

        public static readonly BindableProperty RefreshCommandProperty =
            BindableProperty.Create(nameof(RefreshCommand),
                typeof(ICommand),
                typeof(ContentPageBase));

        public static readonly BindableProperty AppBackgroundImageProperty =
            BindableProperty.Create(nameof(AppBackgroundImage),
                typeof(string),
                typeof(ContentPageBase),
                defaultValue: default(string),
                propertyChanged: (bindable, oldVal, newVal) => (bindable as ContentPageBase)?.OnAppBackgroundImage());

        private TapGestureRecognizer _popupBlockBackingTapGesture = new TapGestureRecognizer();
        private TapGestureRecognizer _bottomItemTapGestureRecognizer = new TapGestureRecognizer();
        private Dictionary<NavigationModeBase, Grid> _bottomBarModeViewPairs = new Dictionary<NavigationModeBase, Grid>();

        public ContentPageBase() {
            InitializeComponent();

            _popupBlockBackingTapGesture.Command = new Command(() => {
                IsPopupVisible = false;
            });
            _bottomItemTapGestureRecognizer.Tapped += OnBottomItemTapGestureRecognizerTapped;

            SetBinding(IsBusyAwaitingProperty, new Binding(_BUSY_BINDING_PATH));
            SetBinding(IsPopupVisibleProperty, new Binding(_IS_POPUP_VISIBLE_BINDING_PATH, BindingMode.TwoWay));
            SetBinding(AppBackgroundImageProperty, new Binding(_APP_BACKGROUND_IMAGE_BINDING_PATH, BindingMode.OneWay));
            SetBinding(NavigationModesProperty, new Binding(_NAVIGATION_MODES_BINDING_PATH, BindingMode.OneWay));
            SetBinding(SelectedNavigationModeProperty, new Binding(_SELECTED_NAVIGATION_MODE_BINDING_PATH, BindingMode.TwoWay));
            SetBinding(SelectedBottomItemIndexProperty, new Binding(_SELECTED_BOTTOM_ITEM_INDEX_BINDING_PATH, BindingMode.TwoWay, source: this));

            ///
            /// Temporay disabled features
            /// 
            //SetBinding(IsPullToRefreshEnabledProperty, new Binding(_IS_PULL_TO_REFRESH_ENABLED_BINDING_PATH, BindingMode.OneWay));
            //SetBinding(IsRefreshingProperty, new Binding(_IS_REFRESHING_BINDING_PATH, BindingMode.TwoWay));
            //SetBinding(RefreshCommandProperty, new Binding(_REFRESH_COMMAND_BINDING_PATH, BindingMode.OneWay));

            _mainContentSpot_PullToRefreshLayout.SetBinding(PullToRefreshLayout.IsPullToRefreshEnabledProperty, new Binding(_IS_PULL_TO_REFRESH_ENABLED_BINDING_PATH, mode: BindingMode.OneWay, source: this));
            _mainContentSpot_PullToRefreshLayout.SetBinding(PullToRefreshLayout.IsRefreshingProperty, new Binding(_IS_REFRESHING_BINDING_PATH, mode: BindingMode.TwoWay, source: this));
            _mainContentSpot_PullToRefreshLayout.SetBinding(PullToRefreshLayout.RefreshCommandProperty, new Binding(_REFRESH_COMMAND_BINDING_PATH, mode: BindingMode.OneWay, source: this));

            _popupsKeeper_PopupsBlockView.SetBinding(PopupsBlockView.PopupsProperty, new Binding(_POPUPS_BINDING_PATH, mode: BindingMode.OneWay));
            _popupsKeeper_PopupsBlockView.Backing.GestureRecognizers.Add(_popupBlockBackingTapGesture);

            OnAppBackgroundImage();
        }

        public int SelectedBottomItemIndex {
            get => (int)GetValue(SelectedBottomItemIndexProperty);
            set => SetValue(SelectedBottomItemIndexProperty, value);
        }

        public string AppBackgroundImage {
            get => GetValue(AppBackgroundImageProperty) as string;
            set => SetValue(AppBackgroundImageProperty, value);
        }

        public ICommand RefreshCommand {
            get { return (ICommand)GetValue(ContentPageBase.RefreshCommandProperty); }
            set { SetValue(ContentPageBase.RefreshCommandProperty, value); }
        }

        public NavigationModeBase SelectedNavigationMode {
            get => (NavigationModeBase)GetValue(SelectedNavigationModeProperty);
            set => SetValue(SelectedNavigationModeProperty, value);
        }

        public bool IsRefreshing {
            get => (bool)GetValue(ContentPageBase.IsRefreshingProperty);
            set => SetValue(ContentPageBase.IsRefreshingProperty, value);
        }

        public bool IsPullToRefreshEnabled {
            get => (bool)GetValue(ContentPageBase.IsPullToRefreshEnabledProperty);
            set => SetValue(ContentPageBase.IsPullToRefreshEnabledProperty, value);
        }

        public IEnumerable<NavigationModeBase> NavigationModes {
            get => (IEnumerable<NavigationModeBase>)GetValue(NavigationModesProperty);
            set => SetValue(NavigationModesProperty, value);
        }

        public bool IsPopupVisible {
            get => (bool)GetValue(ContentPageBase.IsPopupVisibleProperty);
            set => SetValue(ContentPageBase.IsPopupVisibleProperty, value);
        }

        public View MainContent {
            get => (View)GetValue(ContentPageBase.MainContentProperty);
            set => SetValue(ContentPageBase.MainContentProperty, value);
        }

        public ActionBarBase ActionBar {
            get => (ActionBarBase)GetValue(ContentPageBase.ActionBarProperty);
            set => SetValue(ContentPageBase.ActionBarProperty, value);
        }

        public bool IsBusyAwaiting {
            get => (bool)GetValue(ContentPageBase.IsBusyAwaitingProperty);
            set => SetValue(ContentPageBase.IsBusyAwaitingProperty, value);
        }

        private void OnAppBackgroundImage() {
            if (string.IsNullOrEmpty(AppBackgroundImage) || string.IsNullOrWhiteSpace(AppBackgroundImage)) {
                Grid.SetRow(_appBackgroundImageSpot_Grid, 1);
                _backgroundImage_CachedImage.Source = null;
            }
            else {
                try {
                    Grid.SetRow(_appBackgroundImageSpot_Grid, 0);
                    _backgroundImage_CachedImage.Source = ImageSource.FromUri(new Uri(AppBackgroundImage));
                }
                catch {
                    Grid.SetRow(_appBackgroundImageSpot_Grid, 1);
                    _backgroundImage_CachedImage.Source = null;
                }
            }
        }

        private void OnSelectedBottomItemIndex() {
            foreach (NavigationModeBase modeContext in _bottomBarModeViewPairs.Keys) {
                Grid targetBottomItemsGrid = _bottomBarModeViewPairs[modeContext];
                SingleBottomItem[] items = targetBottomItemsGrid.Children.OfType<SingleBottomItem>().ToArray();

                if (modeContext == SelectedNavigationMode) {
                    for (int i = 0; i < items.Count(); i++) {
                        items.ElementAt(i).IsSelected = (i == SelectedBottomItemIndex);
                        
                        View mainContent = items.ElementAt(i).AppropriateItemContentView;

                        if ((items.ElementAt(i).IsSelected)) {
                            if (!_contentBox_Grid.Children.Contains(mainContent)) {
                                _contentBox_Grid.Children.Add(mainContent);
                            }

                            mainContent.TranslationX = 0;
                        }
                        else {
                            mainContent.TranslationX = short.MaxValue;
                        }
                    }
                }
                else {
                    for (int i = 0; i < items.Count(); i++) {
                        items.ElementAt(i).IsSelected = false;

                        items.ElementAt(i).AppropriateItemContentView.TranslationX = short.MaxValue;
                    }
                }
            }
        }

        private void OnBottomItemTapGestureRecognizerTapped(object sender, EventArgs e) {
            if (SelectedBottomItemIndex != ((SingleBottomItem)sender).TabIndex) {
                SelectedBottomItemIndex = ((SingleBottomItem)sender).TabIndex;
            }
        }
    }
}




















//using PeakMVP.Controls;
//using PeakMVP.Controls.ActionBars.Base;
//using PeakMVP.Controls.Menus.Base;
//using PeakMVP.Controls.Popups;
//using PeakMVP.Models.AppNavigation;
//using PeakMVP.ViewModels.Base;
//using PeakMVP.Views.CompoundedViews.MainContent;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Windows.Input;
//using Xamarin.Forms;
//using Xamarin.Forms.Internals;
//using Xamarin.Forms.Xaml;

//namespace PeakMVP.Views.Base {
//    [XamlCompilation(XamlCompilationOptions.Compile)]
//    public abstract partial class ContentPageBase : ContentPage {

//        private static readonly string _BUSY_BINDING_PATH = "IsBusy";
//        private static readonly string _IS_POPUP_VISIBLE_BINDING_PATH = "IsPopupsVisible";
//        private static readonly string _APP_BACKGROUND_IMAGE_BINDING_PATH = "AppBackgroundImage";
//        private static readonly string _POPUPS_BINDING_PATH = "Popups";
//        private static readonly string _NAVIGATION_MODES_BINDING_PATH = "ActionBarViewModel.Modes";
//        private static readonly string _SELECTED_NAVIGATION_MODE_BINDING_PATH = "ActionBarViewModel.SelectedMode";
//        private static readonly string _SELECTED_BOTTOM_ITEM_INDEX_BINDING_PATH = "SelectedNavigationMode.SelectedBarItemIndex";

//        private static readonly string _IS_PULL_TO_REFRESH_ENABLED_BINDING_PATH = "IsPullToRefreshEnabled";
//        private static readonly string _IS_REFRESHING_BINDING_PATH = "IsRefreshing";
//        private static readonly string _REFRESH_COMMAND_BINDING_PATH = "RefreshCommand";

//        public static readonly BindableProperty MainContentProperty =
//            BindableProperty.Create(nameof(MainContent),
//                typeof(View),
//                typeof(ContentPageBase),
//                defaultValue: null,
//                propertyChanged: (BindableObject bindable, object oldValue, object newValue) => {
//                    if (bindable is ContentPageBase declarer) {
//                        declarer._contentBox_Grid.Children.Add(newValue as View);
//                    }
//                });

//        public static readonly BindableProperty ActionBarProperty =
//            BindableProperty.Create(nameof(ActionBar),
//                typeof(ActionBarBase),
//                typeof(ContentPageBase),
//                defaultValue: null,
//                propertyChanged: (BindableObject bindable, object oldValue, object newValue) => {
//                    if (bindable is ContentPageBase declarer) {
//                        declarer._actionBarSpot_ContentView.Content = newValue as ActionBarBase;
//                    }
//                });

//        public static readonly BindableProperty IsBusyAwaitingProperty =
//            BindableProperty.Create(nameof(IsBusyAwaiting),
//                typeof(bool),
//                typeof(ContentPageBase),
//                defaultValue: false,
//                propertyChanged: (BindableObject bindable, object oldValue, object newValue) => {
//                    if (bindable is ContentPageBase declarer) {
//                        declarer._busyIndicator_Indicator.IsVisible = declarer.IsBusyAwaiting;

//                        if (declarer.IsBusyAwaiting) {
//                            Grid.SetRow(declarer._busyIndicator_Indicator, 0);
//                        }
//                        else {
//                            Grid.SetRow(declarer._busyIndicator_Indicator, 1);
//                        }
//                    }
//                });

//        public static readonly BindableProperty IsPopupVisibleProperty =
//            BindableProperty.Create(nameof(IsPopupVisible),
//                typeof(bool),
//                typeof(ContentPageBase),
//                defaultValue: false,
//                propertyChanged: async (BindableObject bindable, object oldValue, object newValue) => {
//                    if (bindable is ContentPageBase declarer) {
//                        if (declarer.IsPopupVisible) {
//                            declarer._popupSpot_ContentView.Opacity = 0;
//                            Grid.SetRow(declarer._popupSpot_ContentView, 0);
//                            await declarer._popupSpot_ContentView.FadeTo(1);
//                        }
//                        else {
//                            declarer._popupSpot_ContentView.Opacity = 1;
//                            await declarer._popupSpot_ContentView.FadeTo(0);
//                            Grid.SetRow(declarer._popupSpot_ContentView, 1);

//                            declarer._popupsKeeper_PopupsBlockView.CloseAllPopups();
//                        }
//                    }
//                });

//        public static readonly BindableProperty NavigationModesProperty =
//            BindableProperty.Create(nameof(NavigationModes),
//                typeof(IReadOnlyCollection<NavigationModeBase>),
//                typeof(ContentPageBase),
//                defaultValue: null,
//                propertyChanged: (BindableObject bindable, object oldValue, object newValue) => {
//                    if (bindable is ContentPageBase declarer) {
//                        declarer._bottomBarSpot_Grid.Children.Clear();
//                        declarer._bottomBarSpot_Grid.ColumnDefinitions.Clear();
//                        declarer._contentBox_Grid.Children.Clear();

//                        declarer._bottomBarModeViewPairs.Clear();

//                        if (declarer.NavigationModes != null) {
//                            foreach (NavigationModeBase modeItem in declarer.NavigationModes) {
//                                Grid bottomItemsRow = new Grid();
//                                bottomItemsRow.ColumnSpacing = 0;

//                                for (int i = 0; i < modeItem.BarItems.Count; i++) {
//                                    SingleBottomItem singleVisualBottomItem = new SingleBottomItem();
//                                    singleVisualBottomItem.TabIndex = i;
//                                    singleVisualBottomItem.BindingContext = modeItem.BarItems.ElementAt(i);
//                                    singleVisualBottomItem.GestureRecognizers.Add(declarer._bottomItemTapGestureRecognizer);
//                                    Grid.SetColumn(singleVisualBottomItem, i);

//                                    bottomItemsRow.ColumnDefinitions.Add(new ColumnDefinition() {
//                                        Width = new GridLength(1, GridUnitType.Star)
//                                    });
//                                    bottomItemsRow.Children.Add(singleVisualBottomItem);

//                                    declarer._contentBox_Grid.Children.Add(singleVisualBottomItem.AppropriateItemContentView);
//                                    declarer._bottomBarSpot_Grid.Children.Add(bottomItemsRow);
//                                }

//                                declarer._bottomBarModeViewPairs.Add(modeItem, bottomItemsRow);
//                            }
//                        }
//                    }
//                });

//        public static readonly BindableProperty SelectedNavigationModeProperty =
//            BindableProperty.Create(nameof(SelectedNavigationMode),
//                typeof(NavigationModeBase),
//                typeof(ContentPageBase),
//                defaultValue: null,
//                propertyChanged: (BindableObject bindable, object oldValue, object newValue) => {
//                    if (bindable is ContentPageBase declarer) {
//                        Grid requiredBottomItemsGrid = declarer._bottomBarModeViewPairs[declarer.SelectedNavigationMode];

//                        declarer._bottomBarSpot_Grid.Children.ForEach(c => c.TranslationX = (c == requiredBottomItemsGrid) ? 0 : short.MaxValue);
//                        declarer.SelectedBottomItemIndex = 0;
//                        if (declarer.SelectedBottomItemIndex != 0) {
//                            declarer.SelectedBottomItemIndex = 0;
//                        }
//                        else {
//                            declarer.OnSelectedBottomItemIndex();
//                        }

//                        if (oldValue != null) {
//                            ((NavigationModeBase)oldValue).SelectedBarItemIndex = 0;
//                        }
//                    }
//                });

//        public static readonly BindableProperty SelectedBottomItemIndexProperty =
//            BindableProperty.Create(nameof(SelectedBottomItemIndex),
//                typeof(int),
//                typeof(ContentPageBase),
//                defaultValue: -1,
//                propertyChanged: (BindableObject bindable, object oldValue, object newValue) => (bindable as ContentPageBase)?.OnSelectedBottomItemIndex());

//        public static readonly BindableProperty IsPullToRefreshEnabledProperty =
//            BindableProperty.Create(nameof(IsPullToRefreshEnabled),
//                typeof(bool),
//                typeof(ContentPageBase),
//                defaultValue: default(bool));

//        public static readonly BindableProperty IsRefreshingProperty =
//            BindableProperty.Create(nameof(IsRefreshing),
//                typeof(bool),
//                typeof(ContentPageBase),
//                defaultValue: default(bool));

//        public static readonly BindableProperty RefreshCommandProperty =
//            BindableProperty.Create(nameof(RefreshCommand),
//                typeof(ICommand),
//                typeof(ContentPageBase));

//        public static readonly BindableProperty AppBackgroundImageProperty =
//            BindableProperty.Create(nameof(AppBackgroundImage),
//                typeof(string),
//                typeof(ContentPageBase),
//                defaultValue: default(string),
//                propertyChanged: (bindable, oldVal, newVal) => (bindable as ContentPageBase)?.OnAppBackgroundImage());

//        private TapGestureRecognizer _popupBlockBackingTapGesture = new TapGestureRecognizer();
//        private TapGestureRecognizer _bottomItemTapGestureRecognizer = new TapGestureRecognizer();
//        private Dictionary<NavigationModeBase, Grid> _bottomBarModeViewPairs = new Dictionary<NavigationModeBase, Grid>();

//        public ContentPageBase() {
//            InitializeComponent();

//            _popupBlockBackingTapGesture.Command = new Command(() => {
//                IsPopupVisible = false;
//            });
//            _bottomItemTapGestureRecognizer.Tapped += OnBottomItemTapGestureRecognizerTapped;

//            SetBinding(IsBusyAwaitingProperty, new Binding(_BUSY_BINDING_PATH));
//            SetBinding(IsPopupVisibleProperty, new Binding(_IS_POPUP_VISIBLE_BINDING_PATH, BindingMode.TwoWay));
//            SetBinding(AppBackgroundImageProperty, new Binding(_APP_BACKGROUND_IMAGE_BINDING_PATH, BindingMode.OneWay));
//            SetBinding(NavigationModesProperty, new Binding(_NAVIGATION_MODES_BINDING_PATH, BindingMode.OneWay));
//            SetBinding(SelectedNavigationModeProperty, new Binding(_SELECTED_NAVIGATION_MODE_BINDING_PATH, BindingMode.TwoWay));
//            SetBinding(SelectedBottomItemIndexProperty, new Binding(_SELECTED_BOTTOM_ITEM_INDEX_BINDING_PATH, BindingMode.TwoWay, source: this));
//            SetBinding(IsPullToRefreshEnabledProperty, new Binding(_IS_PULL_TO_REFRESH_ENABLED_BINDING_PATH, BindingMode.OneWay));
//            SetBinding(IsRefreshingProperty, new Binding(_IS_REFRESHING_BINDING_PATH, BindingMode.TwoWay));
//            SetBinding(RefreshCommandProperty, new Binding(_REFRESH_COMMAND_BINDING_PATH, BindingMode.OneWay));

//            _mainContentSpot_PullToRefreshLayout.SetBinding(PullToRefreshLayout.IsPullToRefreshEnabledProperty, new Binding(_IS_PULL_TO_REFRESH_ENABLED_BINDING_PATH, mode: BindingMode.OneWay, source: this));
//            _mainContentSpot_PullToRefreshLayout.SetBinding(PullToRefreshLayout.IsRefreshingProperty, new Binding(_IS_REFRESHING_BINDING_PATH, mode: BindingMode.TwoWay, source: this));
//            _mainContentSpot_PullToRefreshLayout.SetBinding(PullToRefreshLayout.RefreshCommandProperty, new Binding(_REFRESH_COMMAND_BINDING_PATH, mode: BindingMode.OneWay, source: this));

//            _popupsKeeper_PopupsBlockView.SetBinding(PopupsBlockView.PopupsProperty, new Binding(_POPUPS_BINDING_PATH, mode: BindingMode.OneWay));
//            _popupsKeeper_PopupsBlockView.Backing.GestureRecognizers.Add(_popupBlockBackingTapGesture);

//            OnAppBackgroundImage();
//        }

//        public int SelectedBottomItemIndex {
//            get => (int)GetValue(SelectedBottomItemIndexProperty);
//            set => SetValue(SelectedBottomItemIndexProperty, value);
//        }

//        public string AppBackgroundImage {
//            get => GetValue(AppBackgroundImageProperty) as string;
//            set => SetValue(AppBackgroundImageProperty, value);
//        }

//        public ICommand RefreshCommand {
//            get { return (ICommand)GetValue(ContentPageBase.RefreshCommandProperty); }
//            set { SetValue(ContentPageBase.RefreshCommandProperty, value); }
//        }

//        public NavigationModeBase SelectedNavigationMode {
//            get => (NavigationModeBase)GetValue(SelectedNavigationModeProperty);
//            set => SetValue(SelectedNavigationModeProperty, value);
//        }

//        public bool IsRefreshing {
//            get => (bool)GetValue(ContentPageBase.IsRefreshingProperty);
//            set => SetValue(ContentPageBase.IsRefreshingProperty, value);
//        }

//        public bool IsPullToRefreshEnabled {
//            get => (bool)GetValue(ContentPageBase.IsPullToRefreshEnabledProperty);
//            set => SetValue(ContentPageBase.IsPullToRefreshEnabledProperty, value);
//        }

//        public IEnumerable<NavigationModeBase> NavigationModes {
//            get => (IEnumerable<NavigationModeBase>)GetValue(NavigationModesProperty);
//            set => SetValue(NavigationModesProperty, value);
//        }

//        public bool IsPopupVisible {
//            get => (bool)GetValue(ContentPageBase.IsPopupVisibleProperty);
//            set => SetValue(ContentPageBase.IsPopupVisibleProperty, value);
//        }

//        public View MainContent {
//            get => (View)GetValue(ContentPageBase.MainContentProperty);
//            set => SetValue(ContentPageBase.MainContentProperty, value);
//        }

//        public ActionBarBase ActionBar {
//            get => (ActionBarBase)GetValue(ContentPageBase.ActionBarProperty);
//            set => SetValue(ContentPageBase.ActionBarProperty, value);
//        }

//        public bool IsBusyAwaiting {
//            get => (bool)GetValue(ContentPageBase.IsBusyAwaitingProperty);
//            set => SetValue(ContentPageBase.IsBusyAwaitingProperty, value);
//        }

//        private void OnAppBackgroundImage() {
//            if (string.IsNullOrEmpty(AppBackgroundImage) || string.IsNullOrWhiteSpace(AppBackgroundImage)) {
//                Grid.SetRow(_appBackgroundImageSpot_Grid, 1);
//                _backgroundImage_CachedImage.Source = null;
//            }
//            else {
//                try {
//                    Grid.SetRow(_appBackgroundImageSpot_Grid, 0);
//                    _backgroundImage_CachedImage.Source = ImageSource.FromUri(new Uri(AppBackgroundImage));
//                }
//                catch {
//                    Grid.SetRow(_appBackgroundImageSpot_Grid, 1);
//                    _backgroundImage_CachedImage.Source = null;
//                }
//            }
//        }

//        private void OnSelectedBottomItemIndex() {
//            foreach (NavigationModeBase modeContext in _bottomBarModeViewPairs.Keys) {
//                Grid targetBottomItemsGrid = _bottomBarModeViewPairs[modeContext];
//                SingleBottomItem[] items = targetBottomItemsGrid.Children.OfType<SingleBottomItem>().ToArray();

//                if (modeContext == SelectedNavigationMode) {
//                    for (int i = 0; i < items.Count(); i++) {
//                        items.ElementAt(i).IsSelected = (i == SelectedBottomItemIndex);

//                        items.ElementAt(i).AppropriateItemContentView.TranslationX = (items.ElementAt(i).IsSelected) ? 0 : short.MaxValue;
//                    }
//                }
//                else {
//                    for (int i = 0; i < items.Count(); i++) {
//                        items.ElementAt(i).IsSelected = false;

//                        items.ElementAt(i).AppropriateItemContentView.TranslationX = short.MaxValue;
//                    }
//                }
//            }
//        }

//        private void OnBottomItemTapGestureRecognizerTapped(object sender, EventArgs e) {
//            if (SelectedBottomItemIndex != ((SingleBottomItem)sender).TabIndex) {
//                SelectedBottomItemIndex = ((SingleBottomItem)sender).TabIndex;
//            }
//        }
//    }
//}

