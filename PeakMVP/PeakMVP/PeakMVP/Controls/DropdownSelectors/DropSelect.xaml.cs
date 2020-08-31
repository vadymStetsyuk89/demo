using PeakMVP.Controls.DropdownSelectors.Base;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PeakMVP.Controls.DropdownSelectors {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DropSelect : DropdownSelectorBase {

        private static readonly double _HIDE_HINT_Y_OFFSET = double.MaxValue;

        private View _hintView;

        public DropSelect() {
            InitializeComponent();

            _contentWraper_ExtendedContentView.SetBinding(ExtendedContentView.BorderColorProperty, new Binding(DropdownSelectorBase._BORDER_COLOR_PROPERTY_PATH, source: this));
        }

        public bool SuggestionsOnlyForMultipleItems { get; set; }

        private View _selectedItemFiguring;
        public View SelectedItemFiguring {
            get => _selectedItemFiguring;
            set {
                if (_selectedItemFiguring != null) {
                    _selectedItemFiguring.BindingContext = null;
                    if (_selectedItemFiguring.Parent != null) {
                        _mainContentSpot_AbsoluteLayout.Children.Remove(_selectedItemFiguring);
                    }
                }

                _selectedItemFiguring = value;

                if (_selectedItemFiguring != null) {
                    _selectedItemFiguring.BindingContext = SelectedItem;

                    AbsoluteLayout.SetLayoutFlags(_selectedItemFiguring, AbsoluteLayoutFlags.All);
                    AbsoluteLayout.SetLayoutBounds(_selectedItemFiguring, new Rectangle(1, 1, 1, 1));

                    UpdateVisibilityOfSelectedItem();
                }
            }
        }

        private DataTemplate _hintViewTemplate;
        public DataTemplate HintViewTemplate {
            get => _hintViewTemplate;
            set {
                _hintViewTemplate = value;

                ApplyHintView();
                ApplyHint();
            }
        }

        private DataTemplate _buttonViewTemplate;
        public DataTemplate ButtonViewTemplate {
            get => _buttonViewTemplate;
            set {
                _buttonViewTemplate = value;

                if (_buttonViewTemplate != null) {
                    try {
                        _buttonSpot_ContentView.Content = (View)value.CreateContent();
                    }
                    catch (Exception exc) {
                        throw new InvalidOperationException("DropSelect ButtonViewTemplate setter", exc);
                    }
                }
            }
        }

        private bool _isHintEnabled;
        public bool IsHintEnabled {
            get => _isHintEnabled;
            set {
                _isHintEnabled = value;

                ApplyHint();
            }
        }

        private bool _isButtonEnabled;
        public bool IsButtonEnabled {
            get => _isButtonEnabled;
            set {
                _isButtonEnabled = value;

                _buttonSpot_ContentView.IsVisible = value;
            }
        }

        public bool FirstElementAsDefault { get; set; }

        protected override void OnItemSelected() {
            base.OnItemSelected();

            UpdateVisibilityOfSelectedItem();
            ResolveDropChevron();
        }

        protected override void OnItemSourceApplayed() {
            //
            // Override without anything will prevent popover 'automatically' displaying when item-source value changed. 
            //
            if (FirstElementAsDefault && ItemSource != null && ItemSource.Count > 0) {
                SelectedItem = ItemSource[0];
            }

            ResolveDropChevron();
        }

        private void ApplyHintView() {
            _hintView = HintViewTemplate?.CreateContent() as View;

            _hintScope_ContentView.Content = _hintView;
        }

        private void ApplyHint() {
            _hintScope_ContentView.IsVisible = IsHintEnabled;

            double yOffset = (SelectedItem != null) ? DropSelect._HIDE_HINT_Y_OFFSET : 0;
            _hintScope_ContentView.TranslationY = yOffset;
        }

        private void UpdateVisibilityOfSelectedItem() {
            ApplyHint();

            if (SelectedItem != null) {

                SelectedItemFiguring.BindingContext = SelectedItem;

                if (SelectedItemFiguring.Parent == null) {
                    _mainContentSpot_AbsoluteLayout.Children.Add(SelectedItemFiguring);
                }
            }
            else {
                _mainContentSpot_AbsoluteLayout.Children.Remove(SelectedItemFiguring);
            }
        }

        private void OnMainContentSpotTap(object sender, EventArgs e) {
            if (ItemSource != null && ItemSource.Count > 0) {
                if (SuggestionsOnlyForMultipleItems) {
                    if (SelectedItem == null || ItemSource.Count > 1) {
                        ShowPopover();
                    }
                }
                else {
                    ShowPopover();
                }
            }
        }

        private void ResolveDropChevron() {
            if (SuggestionsOnlyForMultipleItems) {
                if (ItemSource != null && ItemSource.Count == 1 && SelectedItem != null) {
                    _chevronDrop_SvgCachedImage.Opacity = 0;
                }
                else {
                    _chevronDrop_SvgCachedImage.Opacity = 1;
                }
            }
            else {
                _chevronDrop_SvgCachedImage.Opacity = 1;
            }
        }
    }
}