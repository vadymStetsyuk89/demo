using PeakMVP.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace PeakMVP.Controls {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContentSwitcher : ContentView {

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

                        if (newValue != null) {
                            IEnumerable<IVisualFiguring> newValueList = ((IEnumerable<object>)newValue).OfType<IVisualFiguring>();

                            newValueList.ForEach(iVF => {
                                declarer._contextAndViewPairs.Add(iVF, null);
                            });

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
            propertyChanged: (BindableObject bindable, object oldValue, object newValue) => {
                if (bindable is ContentSwitcher declarer) {
                    try {
                        for (int i = 0; i < declarer._contextAndViewPairs.Count; i++) {
                            IVisualFiguring targetContext = declarer._contextAndViewPairs.Keys.ElementAt(i);
                            View targetContextView = declarer._contextAndViewPairs[targetContext];

                            if (((int)newValue) == declarer._contextAndViewPairs.Keys.IndexOf(targetContext)) {
                                if (targetContextView == null) {
                                    targetContextView = (View)new DataTemplate(targetContext.RelativeViewType).CreateContent();
                                    targetContextView.BindingContext = targetContext;

                                    declarer._contextAndViewPairs[targetContext] = targetContextView;

                                    declarer._mainContentSpot_Grid.Children.Add(targetContextView);
                                }

                                declarer.ChangeContentVisibility(targetContextView, true);
                            }
                            else {
                                if (targetContextView != null) {
                                    declarer.ChangeContentVisibility(targetContextView, false);
                                }
                            }
                        }
                    }
                    catch (Exception exc) {
                        Debugger.Break();
                        throw new InvalidOperationException("ContentSwitcher on SelectedContentItemIndexProperty property changed.", exc);
                    }
                }
            });

        private Dictionary<IVisualFiguring, View> _contextAndViewPairs = new Dictionary<IVisualFiguring, View>();

        public ContentSwitcher() {
            InitializeComponent();
        }

        public IEnumerable<object> ContentItemSource {
            get => (IEnumerable<object>)GetValue(ContentItemSourceProperty);
            set => SetValue(ContentItemSourceProperty, value);
        }

        public int SelectedContentItemIndex {
            get => (int)GetValue(SelectedContentItemIndexProperty);
            set => SetValue(SelectedContentItemIndexProperty, value);
        }

        private void ChangeContentVisibility(View targetView, bool makeVisible) {
            targetView.TranslationY = makeVisible ? 0 : short.MaxValue;
            Grid.SetRow(targetView, makeVisible ? _MAIN_VISUAL_ROW_INDEX : _NICHE_ROW_INDEX);
        }
    }
}