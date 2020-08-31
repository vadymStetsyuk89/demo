using PeakMVP.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace PeakMVP.Controls.Switcher {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContentSwitcher : ContentView {

        private static readonly Color _MAIN_SELECT_COLOR_RESOURCE_KEY = (Color)App.Current.Resources["WhiteColor"];
        private static readonly Color _MAIN_UNSELECT_COLOR_RESOURCE_KEY = (Color)App.Current.Resources["SemiLightGrayColor"];
        private static readonly double _Y_SPACE_FOR_RUNNER = -2;

        private static int _NICHE_ROW_INDEX = 0;
        private static int _MAIN_VISUAL_ROW_INDEX = 1;

        public static readonly BindableProperty ContentItemSourceProperty = BindableProperty.Create(
            propertyName: nameof(ContentItemSource),
            returnType: typeof(IEnumerable<object>),
            declaringType: typeof(ContentSwitcher),
            defaultValue: default(IEnumerable<object>),
            propertyChanged: (BindableObject bindable, object oldValue, object newValue) => {
                if (bindable is ContentSwitcher declarer) {
                    try {
                        declarer._mainContentSpot_Grid.Children.Clear();
                        declarer._contextAndViewPairs.Clear();

                        declarer._tassels.ForEach(tassel => {
                            tassel.GestureRecognizers.Clear();
                            declarer._tasseslSpot_Grid.Children.Remove(tassel);
                        });
                        declarer._tassels.Clear();
                        declarer._tasseslSpot_Grid.ColumnDefinitions.Clear();

                        if (newValue != null) {
                            IEnumerable<IVisualFiguring> newValueList = ((IEnumerable<object>)newValue).OfType<IVisualFiguring>();

                            for (int i = 0; i < newValueList.Count(); i++) {
                                declarer._contextAndViewPairs.Add(newValueList.ElementAt(i), null);

                                ///
                                /// Tassel configuring. Separate this action
                                /// 
                                declarer._tasseslSpot_Grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Star });
                                Tassel tassel = new Tassel();
                                tassel.TranslationY = _Y_SPACE_FOR_RUNNER;
                                tassel.SelectedMainColor = _MAIN_SELECT_COLOR_RESOURCE_KEY;
                                tassel.UnSelectedMainColor = _MAIN_UNSELECT_COLOR_RESOURCE_KEY;
                                tassel.BindingContext = newValueList.ElementAt<IVisualFiguring>(i);
                                tassel.GestureRecognizers.Add(declarer._tasselTapGestureRecognizer);
                                Grid.SetColumn(tassel, i);
                                declarer._tasseslSpot_Grid.Children.Add(tassel);
                                declarer._tassels.Add(tassel);
                            }

                            declarer.SelectedContentItemIndex = 0;
                        }
                    }
                    catch (Exception exc) {
                        Debugger.Break();
                        throw new InvalidOperationException("ContentSwitcher on ContentItemSourceProperty property changed.", exc);
                    }
                }
            });

        public static readonly BindableProperty SelectedContentItemIndexProperty = BindableProperty.Create(
            nameof(SelectedContentItemIndex),
            typeof(int),
            typeof(ContentSwitcher),
            defaultValue: -1,
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: (BindableObject bindable, object oldValue, object newValue) => {
                if (bindable is ContentSwitcher declarer) {
                    try {
                        for (int i = 0; i < declarer._contextAndViewPairs.Count; i++) {
                            IVisualFiguring targetContext = declarer._contextAndViewPairs.Keys.ElementAt(i);
                            View targetContextView = declarer._contextAndViewPairs[targetContext];

                            //if (targetContext is ISwitchTab clearedAfterTabTap) {
                            //    clearedAfterTabTap.ClearAfterTabTap();
                            //}

                            if (((int)newValue) == declarer._contextAndViewPairs.Keys.IndexOf(targetContext)) {
                                ///
                                /// Rendering view if is not used yet
                                /// 
                                if (targetContextView == null) {
                                    targetContextView = (View)new DataTemplate(targetContext.RelativeViewType).CreateContent();
                                    targetContextView.BindingContext = targetContext;

                                    declarer._contextAndViewPairs[targetContext] = targetContextView;

                                    declarer._mainContentSpot_Grid.Children.Add(targetContextView);
                                }

                                //if (targetContext is ISwitchTab newClearedAfterTabTap) {
                                //    newClearedAfterTabTap.TabClicked();
                                //}

                                declarer.ChangeContentVisibility(targetContextView, true);
                            }
                            else {
                                if (targetContextView != null) {
                                    declarer.ChangeContentVisibility(targetContextView, false);
                                }
                            }
                        }

                        declarer.TasselVisualSelection();
                    }
                    catch (Exception exc) {
                        Debugger.Break();
                        throw new InvalidOperationException("ContentSwitcher on SelectedContentItemIndexProperty property changed.", exc);
                    }
                }
            });

        private Dictionary<IVisualFiguring, View> _contextAndViewPairs = new Dictionary<IVisualFiguring, View>();
        private List<Tassel> _tassels = new List<Tassel>();
        private TapGestureRecognizer _tasselTapGestureRecognizer = new TapGestureRecognizer();

        public ContentSwitcher() {
            InitializeComponent();

            _tasselTapGestureRecognizer.Tapped += OnTasselTapGestureRecognizerTapped;
            _selectionRunner_ContentView.RegisterRelativeSwitcher(this);
        }

        private void OnTasselTapGestureRecognizerTapped(object sender, EventArgs e) => SelectedContentItemIndex = _tassels.IndexOf((Tassel)sender);

        public IEnumerable<object> ContentItemSource {
            get => (IEnumerable<object>)GetValue(ContentItemSourceProperty);
            set => SetValue(ContentItemSourceProperty, value);
        }

        public int SelectedContentItemIndex {
            get => (int)GetValue(SelectedContentItemIndexProperty);
            set => SetValue(SelectedContentItemIndexProperty, value);
        }

        public async void TasselVisualSelection() {
            for (int i = 0; i < _tassels.Count; i++) {
                _tassels[i].IsTasselSelected = SelectedContentItemIndex == i;
            }

            await _selectionRunner_ContentView.TranslateTo(SelectedContentItemIndex * _selectionRunner_ContentView.Width, 0, 96);
        }

        private void ChangeContentVisibility(View targetView, bool makeVisible) {
            targetView.TranslationY = makeVisible ? 0 : short.MaxValue;
            Grid.SetRow(targetView, makeVisible ? _MAIN_VISUAL_ROW_INDEX : _NICHE_ROW_INDEX);
        }
    }
}