
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PeakMVP.Controls {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditorCompounded : ContentView {

        public static readonly BindableProperty PlaceholderProperty =
            BindableProperty.Create(nameof(Placeholder),
                typeof(string),
                typeof(EditorCompounded),
                defaultValue: default(string));

        public static readonly BindableProperty InputTextProperty =
            BindableProperty.Create(nameof(InputText),
                typeof(string),
                typeof(EditorCompounded),
                defaultValue: default(string),
                defaultBindingMode: BindingMode.TwoWay,
                propertyChanged: (BindableObject bindable, object oldValue, object newValue) => {
                    if (bindable is EditorCompounded declarer) {
                        declarer.ResolvePlaceholderVisibility();
                    }
                });

        public EditorCompounded() {
            InitializeComponent();

            _placeholder_Label.SetBinding(Label.TextProperty, new Binding(nameof(Placeholder), source: this));
            _input_Editor.SetBinding(Editor.TextProperty, new Binding(nameof(InputText), source: this));
        }

        public string Placeholder {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public string InputText {
            get => (string)GetValue(InputTextProperty);
            set => SetValue(InputTextProperty, value);
        }

        private void OnInputEditorFocused(object sender, FocusEventArgs e) => ResolvePlaceholderVisibility();

        private void OnInputEditorUnfocused(object sender, FocusEventArgs e) => ResolvePlaceholderVisibility();

        private void ResolvePlaceholderVisibility() => _placeholder_Label.IsVisible = string.IsNullOrEmpty(InputText);
    }
}