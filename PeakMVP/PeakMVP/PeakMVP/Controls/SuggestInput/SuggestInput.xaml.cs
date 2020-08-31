using PeakMVP.Controls.DropdownSelectors.Base;
using PeakMVP.Controls.Popovers;
using PeakMVP.Controls.Popovers.Arguments;
using PeakMVP.Controls.Popovers.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PeakMVP.Controls.SuggestInput {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SuggestInput : DropdownSelectorBase {

        public static readonly BindableProperty EntryMarginProperty = BindableProperty.Create(
                nameof(EntryMargin),
                typeof(Thickness),
                typeof(SuggestInput),
                defaultValue: default(Thickness));

        public static readonly BindableProperty TextProperty = BindableProperty.Create(
                nameof(Text),
                typeof(string),
                typeof(SuggestInput),
                defaultValue: null,
                propertyChanged: (bindable, oldValue, newValue) => (bindable as SuggestInput)?.OnTextChanged());

        public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(
                nameof(Placeholder),
                typeof(string),
                typeof(SuggestInput),
                defaultValue: null);

        public static readonly BindableProperty StartSuggestionCommandProperty = BindableProperty.Create(
                nameof(StartSuggestionCommand),
                typeof(ICommand),
                typeof(SuggestInput),
                defaultValue: null);

        public static readonly BindableProperty IsHintEnabledProperty = BindableProperty.Create(
                nameof(IsHintEnabled),
                typeof(bool),
                typeof(SuggestInput),
                defaultValue: false,
                propertyChanged: (bindable, oldValue, newValue) => (bindable as SuggestInput)?.WiringHintToPopover());

        public static readonly BindableProperty HintTextProperty = BindableProperty.Create(
                nameof(HintText),
                typeof(string),
                typeof(SuggestInput),
                defaultValue: null);

        public static readonly BindableProperty IsBusyProperty = BindableProperty.Create(
            nameof(IsBusy),
            typeof(bool),
            typeof(SuggestInput),
            defaultValue: false);

        public SuggestInput() {
            InitializeComponent();

            IsSuggestButtonVisible = _isSuggestButtonVisible;
            
            _contentWraper_ExtendedContentView.SetBinding(ExtendedContentView.BorderColorProperty, new Binding(DropdownSelectorBase._BORDER_COLOR_PROPERTY_PATH, source: this));
            _input_EntryEx.SetBinding(View.MarginProperty, new Binding(nameof(EntryMargin), source: this));
        }

        bool _isSuggestButtonVisible;
        public bool IsSuggestButtonVisible {
            get => _isSuggestButtonVisible;
            set {
                _isSuggestButtonVisible = value;
                _suggestButton_ContentView.IsVisible = _isSuggestButtonVisible;
            }
        }

        public bool IsBusy {
            get => (bool)GetValue(IsBusyProperty);
            set => SetValue(IsBusyProperty, value);
        }

        public Thickness EntryMargin {
            get => (Thickness)GetValue(SuggestInput.EntryMarginProperty);
            set => SetValue(SuggestInput.EntryMarginProperty, value);
        }

        public string HintText {
            get => (string)GetValue(SuggestInput.HintTextProperty);
            set => SetValue(SuggestInput.HintTextProperty, value);
        }

        public bool IsHintEnabled {
            get => (bool)GetValue(SuggestInput.IsHintEnabledProperty);
            set => SetValue(SuggestInput.IsHintEnabledProperty, value);
        }

        public string Text {
            get => (string)GetValue(SuggestInput.TextProperty);
            set => SetValue(SuggestInput.TextProperty, value);
        }

        public string Placeholder {
            get => (string)GetValue(SuggestInput.PlaceholderProperty);
            set => SetValue(SuggestInput.PlaceholderProperty, value);
        }

        public ICommand StartSuggestionCommand {
            get => (ICommand)GetValue(SuggestInput.StartSuggestionCommandProperty);
            set => SetValue(SuggestInput.StartSuggestionCommandProperty, value);
        }

        protected override void OnPopoverChanged() {
            base.OnPopoverChanged();
            WiringHintToPopover();
        }

        protected override void OnItemSelected() {
            //
            // Don't remove it
            // This overriding, (witout base call) will prevent popover closing.
            //
        }

        protected override void OnItemSourceApplayed() {
            base.OnItemSourceApplayed();

            if (IsHintEnabled && !string.IsNullOrEmpty(Text)) {
                ShowPopover();
            }
            else {
                HidePopover();
            }
        }

        private void OnTextChanged() {
            HidePopover();
        }

        private void OnStartSUggestionsTapped(object sender, EventArgs e) {
            if (StartSuggestionCommand != null) {
                StartSuggestionCommand.Execute(null);
            }
        }

        private void WiringHintToPopover() {
            if (Popover != null) {
                if (IsHintEnabled) {
                    Binding hintTextBinding = new Binding("HintText", mode: BindingMode.TwoWay, source: this);

                    ((BindableObject)Popover)
                        .SetBinding(PopoverBase.HintTextProperty, hintTextBinding);
                }
                else {
                    ((BindableObject)Popover).ClearValue(PopoverBase.HintTextProperty);
                }
            }
        }

        private void OnEntryFocus(object sender, FocusEventArgs e) {
            if (ItemSource != null && ItemSource.Count > 0) {
                ShowPopover();
            }
        }
    }
}