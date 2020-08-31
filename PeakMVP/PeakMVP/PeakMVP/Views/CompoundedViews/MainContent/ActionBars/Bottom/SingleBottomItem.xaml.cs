using FFImageLoading.Forms;
using FFImageLoading.Transformations;
using PeakMVP.Models.Arguments.InitializeArguments;
using PeakMVP.ViewModels.Base;
using System.Runtime.CompilerServices;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PeakMVP.Views.CompoundedViews.MainContent {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SingleBottomItem : ContentView {

        private static readonly Color _SELECTED_BACKGROUND_COLOR = (Color)App.Current.Resources["BlueColor"];
        private static readonly Color _DESELECTED_BACKGROUND_COLOR = (Color)App.Current.Resources["WhiteColor"];
        private static readonly Color _SELECTED_TITLE_COLOR = (Color)App.Current.Resources["WhiteColor"];
        private static readonly Color _DESELECTED_TITLE_COLOR = (Color)App.Current.Resources["MainBlackColor"];
        private static readonly ColorSpaceTransformation _SELECTED_ICON_COLOR_TRANSFORMATION = new ColorSpaceTransformation(Color.White.ColorToMatrix(1f));

        public BindableProperty IsSelectedProperty = BindableProperty.Create(
            nameof(IsSelected),
            typeof(bool),
            typeof(SingleBottomItem),
            defaultValue: false,
            propertyChanged: (BindableObject bindable, object oldValue, object newValue) => (bindable as SingleBottomItem).OnIsSelected());

        public SingleBottomItem() {
            InitializeComponent();

            SetBinding(IsSelectedProperty, new Binding("IsSelected", mode: BindingMode.TwoWay));

            OnIsSelected();
        }

        public View AppropriateItemContentView { get; private set; }

        public int TabIndex { get; set; }

        public bool IsSelected {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public Color _themeColor;
        public Color ThemeColor {
            get => _themeColor;
            set {
                _themeColor = value;
                _labelText_Label.TextColor = value;
            }
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            base.OnPropertyChanged(propertyName);

            if (propertyName == BindingContextProperty.PropertyName) {
                IBottomBarTab bottomBarItemContext = BindingContext as IBottomBarTab;

                if (bottomBarItemContext != null) {
                    ApplyRelativeView(bottomBarItemContext);
                }
            }
        }

        private void OnIsSelected() {
            if (IsSelected) {
                _rootContainer_Grid.BackgroundColor = _SELECTED_BACKGROUND_COLOR;
                _labelText_Label.TextColor = _SELECTED_TITLE_COLOR;
                _icon_CachedImage.Transformations.Add(_SELECTED_ICON_COLOR_TRANSFORMATION);
                _icon_CachedImage.ReloadImage();

                if (BindingContext is ViewModelBase viewModelBase) {
                    viewModelBase.InitializeAsync(new IntentViewModelArgs());
                }
            }
            else {
                _rootContainer_Grid.BackgroundColor = _DESELECTED_BACKGROUND_COLOR;
                _labelText_Label.TextColor = _DESELECTED_TITLE_COLOR;
                _icon_CachedImage.Transformations.Clear();
                _icon_CachedImage.ReloadImage();

                if (BindingContext is ViewModelBase viewModelBase) {
                    viewModelBase.InitializeAsync(new NoIntentionViewModelArgs());
                }
            }
        }

        private void ApplyRelativeView(IBottomBarTab bottomBarItemContext) {
            AppropriateItemContentView = (View)new DataTemplate(bottomBarItemContext.RelativeViewType).CreateContent();
            AppropriateItemContentView.BindingContext = bottomBarItemContext;
        }
    }
}