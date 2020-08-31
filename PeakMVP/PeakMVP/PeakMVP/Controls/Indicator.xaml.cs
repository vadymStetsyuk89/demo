using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PeakMVP.Controls {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Indicator : ContentView {

        public static readonly BindableProperty PadCanvasColorProperty =
            BindableProperty.Create("PadCanvasColor",
                typeof(Color),
                typeof(Indicator),
                defaultValue: default(Color),
                propertyChanged: OnPadCanvasColorPropertyChanged);

        public static readonly BindableProperty IndicatorColorProperty =
            BindableProperty.Create("IndicatorColor",
                typeof(Color),
                typeof(Indicator),
                defaultValue: default(Color),
                propertyChanged: OnIndicatorColorPropertyChanged);

        public Indicator() {
            InitializeComponent();
        }

        public Color PadCanvasColor {
            get => (Color)GetValue(Indicator.PadCanvasColorProperty);
            set => SetValue(Indicator.PadCanvasColorProperty, value);
        }

        public Color IndicatorColor {
            get => (Color)GetValue(Indicator.IndicatorColorProperty);
            set => SetValue(Indicator.IndicatorColorProperty, value);
        }

        private void ApplyPadCanvasColor() {
            _padCanvas_BoxView.BackgroundColor = PadCanvasColor;
        }

        private void ApplyIndicatorColor() {
            _spinnerIndicator_ActivityIndicator.Color = IndicatorColor;
        }

        private static void OnPadCanvasColorPropertyChanged<PropertyType>(BindableObject bindable, PropertyType oldValue, PropertyType newValue) {
            Indicator indicator = bindable as Indicator;

            if (indicator != null) {
                indicator.ApplyPadCanvasColor();
            }
        }

        private static void OnIndicatorColorPropertyChanged<PropertyType>(BindableObject bindable, PropertyType oldValue, PropertyType newValue) {
            Indicator indicator = bindable as Indicator;

            if (indicator != null) {
                indicator.ApplyIndicatorColor();
            }
        }
    }
}