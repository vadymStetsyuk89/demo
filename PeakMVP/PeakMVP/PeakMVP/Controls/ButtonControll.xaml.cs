using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PeakMVP.Controls {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ButtonControll : ContentView {

        private static readonly string BUTTON_BACKGROUND_COLOR_LUMINOSITY_ANIMATION_NAME = "button_background_color_luminosity_animation_name";

        public static readonly BindableProperty ButtonTextProperty = BindableProperty.Create(
            propertyName: nameof(ButtonText),
            returnType: typeof(string),
            declaringType: typeof(ButtonControll),
            defaultValue: default(string));

        public static readonly BindableProperty ButtonFontSizeProperty = BindableProperty.Create(
            propertyName: nameof(ButtonFontSize),
            returnType: typeof(double),
            declaringType: typeof(ButtonControll),
            defaultValue: default(double));

        public static readonly BindableProperty ButtonTextColorProperty = BindableProperty.Create(
            propertyName: nameof(ButtonTextColor),
            returnType: typeof(Color),
            declaringType: typeof(ButtonControll),
            defaultValue: default(Color));

        public static readonly BindableProperty ButtonFontFamilyProperty = BindableProperty.Create(
            propertyName: nameof(ButtonFontFamily),
            returnType: typeof(string),
            declaringType: typeof(ButtonControll),
            defaultValue: default(string));

        public static readonly BindableProperty ButtonTextHorizontalAlignmentProperty = BindableProperty.Create(
            propertyName: nameof(ButtonTextHorizontalAlignment),
            returnType: typeof(LayoutOptions),
            declaringType: typeof(ButtonControll),
            defaultValue: LayoutOptions.CenterAndExpand);

        public static readonly BindableProperty ButtonTextVerticalAlignmentProperty = BindableProperty.Create(
            propertyName: nameof(ButtonTextVerticalAlignment),
            returnType: typeof(LayoutOptions),
            declaringType: typeof(ButtonControll),
            defaultValue: LayoutOptions.CenterAndExpand);

        public static readonly BindableProperty ButtonBackgroundColorProperty = BindableProperty.Create(
            propertyName: nameof(ButtonBackgroundColor),
            returnType: typeof(Color),
            declaringType: typeof(ButtonControll),
            defaultValue: Color.Transparent,
            propertyChanged: (bindable, oldVal, newVal) => ((ButtonControll)bindable)?.OnButtonBackgroundColor());

        public static readonly BindableProperty ButtonBorderThicknessProperty = BindableProperty.Create(
            propertyName: nameof(ButtonBorderThickness),
            returnType: typeof(int),
            declaringType: typeof(ButtonControll),
            defaultValue: default(int));

        public static readonly BindableProperty ButtonCornerRadiusProperty = BindableProperty.Create(
            propertyName: nameof(ButtonCornerRadius),
            returnType: typeof(int),
            declaringType: typeof(ButtonControll),
            defaultValue: default(int));

        public static readonly BindableProperty ButtonBorderCollorProperty = BindableProperty.Create(
            propertyName: nameof(ButtonBorderCollor),
            returnType: typeof(Color),
            declaringType: typeof(ButtonControll),
            defaultValue: Color.Transparent);

        public static readonly BindableProperty ButtonPaddingProperty = BindableProperty.Create(
            propertyName: nameof(ButtonPadding),
            returnType: typeof(Thickness),
            declaringType: typeof(ButtonControll),
            defaultValue: new Thickness(6));

        public static readonly BindableProperty ButtonCommandProperty = BindableProperty.Create(
            propertyName: nameof(ButtonCommand),
            returnType: typeof(ICommand),
            declaringType: typeof(ButtonControll),
            defaultValue: default(ICommand));

        public static readonly BindableProperty ButtonCommandParameterProperty = BindableProperty.Create(
            propertyName: nameof(ButtonCommandParameter),
            returnType: typeof(object),
            declaringType: typeof(ButtonControll),
            defaultValue: default(object));

        private readonly TapGestureRecognizer _mainContentSpotTapGestureRecognizer = new TapGestureRecognizer();

        public ButtonControll() {
            InitializeComponent();

            ///
            /// Wiring button `text` properties, with appropriate label view
            /// 
            _buttonText_Label.SetBinding(Label.TextProperty, new Binding(nameof(ButtonText), source: this));
            _buttonText_Label.SetBinding(Label.FontSizeProperty, new Binding(nameof(ButtonFontSize), mode: BindingMode.TwoWay, source: this));
            _buttonText_Label.SetBinding(Label.TextColorProperty, new Binding(nameof(ButtonTextColor), mode: BindingMode.TwoWay, source: this));
            _buttonText_Label.SetBinding(Label.FontFamilyProperty, new Binding(nameof(ButtonFontFamily), mode: BindingMode.TwoWay, source: this));
            _buttonText_Label.SetBinding(View.HorizontalOptionsProperty, new Binding(nameof(ButtonTextHorizontalAlignment), mode: BindingMode.TwoWay, source: this));
            _buttonText_Label.SetBinding(View.VerticalOptionsProperty, new Binding(nameof(ButtonTextVerticalAlignment), mode: BindingMode.TwoWay, source: this));

            ///
            /// Wiring button `box` properties, with appropriate container view
            /// 
            _mainContentSpot_ExtendedContentView.SetBinding(VisualElement.BackgroundColorProperty, new Binding(nameof(ButtonBackgroundColor), mode: BindingMode.TwoWay, source: this));
            _mainContentSpot_ExtendedContentView.SetBinding(ExtendedContentView.BorderThicknessProperty, new Binding(nameof(ButtonBorderThickness), mode: BindingMode.TwoWay, source: this));
            _mainContentSpot_ExtendedContentView.SetBinding(ExtendedContentView.CornerRadiusProperty, new Binding(nameof(ButtonCornerRadius), mode: BindingMode.OneWay, source: this));
            _mainContentSpot_ExtendedContentView.SetBinding(ExtendedContentView.BorderColorProperty, new Binding(nameof(ButtonBorderCollor), mode: BindingMode.TwoWay, source: this));
            _content_ContentView.SetBinding(Xamarin.Forms.Layout.PaddingProperty, new Binding(nameof(ButtonPadding), mode: BindingMode.TwoWay, source: this));

            _mainContentSpotTapGestureRecognizer.Command = new Command((object param) => {
                ButtonCommand?.Execute(ButtonCommandParameter);

                this.AbortAnimation(BUTTON_BACKGROUND_COLOR_LUMINOSITY_ANIMATION_NAME);

                new Animation(((v) => _backing_BoxView.Opacity = (v <= .4) ? v : .8 - v), 0, .8)
                    .Commit(this, BUTTON_BACKGROUND_COLOR_LUMINOSITY_ANIMATION_NAME, length: 125, finished: (d, b) => _backing_BoxView.Opacity = 0);
            });
            _mainContentSpot_ExtendedContentView.GestureRecognizers.Add(_mainContentSpotTapGestureRecognizer);
            OnButtonBackgroundColor();
        }

        public ICommand ButtonCommand {
            get => (ICommand)GetValue(ButtonControll.ButtonCommandProperty);
            set => SetValue(ButtonControll.ButtonCommandProperty, value);
        }

        public object ButtonCommandParameter {
            get => GetValue(ButtonCommandParameterProperty);
            set => SetValue(ButtonCommandParameterProperty, value);
        }

        public Thickness ButtonPadding {
            get => (Thickness)GetValue(ButtonControll.ButtonPaddingProperty);
            set => SetValue(ButtonControll.ButtonPaddingProperty, value);
        }

        public Color ButtonBorderCollor {
            get => (Color)GetValue(ButtonControll.ButtonBorderCollorProperty);
            set => SetValue(ButtonControll.ButtonBorderCollorProperty, value);
        }

        public int ButtonCornerRadius {
            get => (int)GetValue(ButtonControll.ButtonCornerRadiusProperty);
            set => SetValue(ButtonControll.ButtonCornerRadiusProperty, value);
        }

        public int ButtonBorderThickness {
            get => (int)GetValue(ButtonControll.ButtonBorderThicknessProperty);
            set => SetValue(ButtonControll.ButtonBorderThicknessProperty, value);
        }

        public Color ButtonBackgroundColor {
            get => (Color)GetValue(ButtonControll.ButtonBackgroundColorProperty);
            set => SetValue(ButtonControll.ButtonBackgroundColorProperty, value);
        }

        public LayoutOptions ButtonTextVerticalAlignment {
            get => (LayoutOptions)GetValue(ButtonControll.ButtonTextVerticalAlignmentProperty);
            set => SetValue(ButtonControll.ButtonTextVerticalAlignmentProperty, value);
        }

        public LayoutOptions ButtonTextHorizontalAlignment {
            get => (LayoutOptions)GetValue(ButtonControll.ButtonTextHorizontalAlignmentProperty);
            set => SetValue(ButtonControll.ButtonTextHorizontalAlignmentProperty, value);
        }

        public string ButtonFontFamily {
            get => GetValue(ButtonControll.ButtonFontFamilyProperty) as string;
            set => SetValue(ButtonControll.ButtonFontFamilyProperty, value);
        }

        public Color ButtonTextColor {
            get => (Color)GetValue(ButtonControll.ButtonTextColorProperty);
            set => SetValue(ButtonControll.ButtonTextColorProperty, value);
        }

        public double ButtonFontSize {
            get => (double)GetValue(ButtonControll.ButtonFontSizeProperty);
            set => SetValue(ButtonControll.ButtonFontSizeProperty, value);
        }

        public string ButtonText {
            get => GetValue(ButtonControll.ButtonTextProperty) as string;
            set => SetValue(ButtonControll.ButtonTextProperty, value);
        }

        private void OnButtonBackgroundColor() {
            //_backing_BoxView.BackgroundColor = ButtonBackgroundColor.WithLuminosity(1 - ButtonBackgroundColor.Luminosity);
            _backing_BoxView.BackgroundColor = (ButtonBackgroundColor.Luminosity <= .5) ? Color.WhiteSmoke : Color.LightGray;
        }
    }
}