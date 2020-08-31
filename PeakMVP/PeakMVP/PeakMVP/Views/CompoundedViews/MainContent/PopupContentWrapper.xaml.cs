using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PeakMVP.Views.CompoundedViews.MainContent {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PopupContentWrapper : ContentView {

        private static readonly Color _DEFAULT_BLUE_COLOR = Color.FromHex("#0CA8F4");
        private static readonly Color _DEFAULT_GRAY_COLOR = Color.FromHex("#868E96");
        private static readonly string _CANCEL_BUTTON_TEXT_PROPERTY_PATH = "CancelButtonText";
        private static readonly string _SUCCEED_BUTTON_TEXT_PROPERTY_PATH = "SucceedButtonText";
        private static readonly string _POPUP_TITLE_TEXT_PROPERTY_PATH = "TitleText";
        private static readonly string _CANCEL_BUTTON_COLOR_PROPERTY_PATH = "CancelButtonColor";
        private static readonly string _SUCCEED_BUTTON_COLOR_PROPERTY_PATH = "SucceedButtonColor";

        public static readonly BindableProperty TitleTextProperty =
            BindableProperty.Create("TitleText",
                typeof(string),
                typeof(PopupContentWrapper),
                defaultValue: null);

        public static readonly BindableProperty CloseCommandProperty =
            BindableProperty.Create("CloseCommand",
                typeof(ICommand),
                typeof(PopupContentWrapper),
                defaultValue: null);

        public static readonly BindableProperty CancelCommandProperty =
            BindableProperty.Create("CancelCommand",
                typeof(ICommand),
                typeof(PopupContentWrapper),
                defaultValue: null);

        public static readonly BindableProperty SucceedCommandProperty =
            BindableProperty.Create("SucceedCommand",
                typeof(ICommand),
                typeof(PopupContentWrapper),
                defaultValue: null);

        public static readonly BindableProperty CancelButtonTextProperty =
            BindableProperty.Create("CancelButtonText",
                typeof(string),
                typeof(PopupContentWrapper),
                defaultValue: null);

        public static readonly BindableProperty SucceedButtonTextProperty =
            BindableProperty.Create("SucceedButtonText",
                typeof(string),
                typeof(PopupContentWrapper),
                defaultValue: null);

        public static readonly BindableProperty CancelButtonColorProperty =
            BindableProperty.Create("CancelButtonColor",
                typeof(Color),
                typeof(PopupContentWrapper),
                defaultValue: _DEFAULT_GRAY_COLOR);

        public static readonly BindableProperty SucceedButtonColorProperty =
            BindableProperty.Create("SucceedButtonColor",
                typeof(Color),
                typeof(PopupContentWrapper),
                defaultValue: _DEFAULT_BLUE_COLOR);

        public PopupContentWrapper() {
            InitializeComponent();

            _cancelText_Label.SetBinding(Label.TextProperty, new Binding(PopupContentWrapper._CANCEL_BUTTON_TEXT_PROPERTY_PATH, source: this));
            _succeedText_Label.SetBinding(Label.TextProperty, new Binding(PopupContentWrapper._SUCCEED_BUTTON_TEXT_PROPERTY_PATH, source: this));
            _popupTitle_Label.SetBinding(Label.TextProperty, new Binding(PopupContentWrapper._POPUP_TITLE_TEXT_PROPERTY_PATH, source: this));

            _cancelButton_ExtendedContentView.SetBinding(VisualElement.BackgroundColorProperty, new Binding(PopupContentWrapper._CANCEL_BUTTON_COLOR_PROPERTY_PATH, source: this));
            _succeedButton_ExtendedContentView.SetBinding(VisualElement.BackgroundColorProperty, new Binding(PopupContentWrapper._SUCCEED_BUTTON_COLOR_PROPERTY_PATH, source: this));
        }

        private View _mainContent;
        public View MainContent {
            get => _mainContent;
            set {
                _mainContent = value;
                _mainContentScope_ContentView.Content = value;
            }
        }

        private bool _isCancelButtonVisible;
        public bool IsCancelButtonVisible {
            get => _isCancelButtonVisible;
            set {
                _isCancelButtonVisible = value;
                _cancelButton_ExtendedContentView.IsVisible = _isCancelButtonVisible;
            }
        }

        private bool _isSucceedButtonVisible;
        public bool IsSucceedButtonVisible {
            get => _isSucceedButtonVisible;
            set {
                _isSucceedButtonVisible = value;
                _succeedButton_ExtendedContentView.IsVisible = _isSucceedButtonVisible;
            }
        }

        public Color CancelButtonColor {
            get => (Color)GetValue(PopupContentWrapper.CancelButtonColorProperty);
            set => SetValue(PopupContentWrapper.CancelButtonColorProperty, value);
        }

        public Color SucceedButtonColor {
            get => (Color)GetValue(PopupContentWrapper.SucceedButtonColorProperty);
            set => SetValue(PopupContentWrapper.SucceedButtonColorProperty, value);
        }

        public string TitleText {
            get => (string)GetValue(PopupContentWrapper.TitleTextProperty);
            set => SetValue(PopupContentWrapper.TitleTextProperty, value);
        }

        public string CancelButtonText {
            get => (string)GetValue(PopupContentWrapper.CancelButtonTextProperty);
            set => SetValue(PopupContentWrapper.CancelButtonTextProperty, value);
        }

        public string SucceedButtonText {
            get => (string)GetValue(PopupContentWrapper.SucceedButtonTextProperty);
            set => SetValue(PopupContentWrapper.SucceedButtonTextProperty, value);
        }

        public ICommand CloseCommand {
            get => (ICommand)GetValue(PopupContentWrapper.CloseCommandProperty);
            set => SetValue(PopupContentWrapper.CloseCommandProperty, value);
        }

        public ICommand CancelCommand {
            get => (ICommand)GetValue(PopupContentWrapper.CancelCommandProperty);
            set => SetValue(PopupContentWrapper.CancelCommandProperty, value);
        }

        public ICommand SucceedCommand {
            get => (ICommand)GetValue(PopupContentWrapper.SucceedCommandProperty);
            set => SetValue(PopupContentWrapper.SucceedCommandProperty, value);
        }

        private void OnCloseButtonTapped(object sender, EventArgs e) {
            if (CloseCommand != null) {
                CloseCommand.Execute(null);
            }
        }

        private void OnCancelTapped(object sender, EventArgs e) {
            if (CancelCommand != null) {
                CancelCommand.Execute(null);
            }
        }

        private void OnSucceedTapped(object sender, EventArgs e) {
            if (SucceedCommand != null) {
                SucceedCommand.Execute(null);
            }
        }
    }
}