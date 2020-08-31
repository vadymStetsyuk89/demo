using PeakMVP.Controls.Carousels.CompoundedCarousel.Base;
using PeakMVP.Controls.Carousels.CompoundedCarousel.Buttons;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PeakMVP.Controls.Carousels.CompoundedCarousel {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CompoundedCarousel : ContentView {

        private static readonly string _CAROUSEL_ITEM_SOURCE_PROPERTY_PATH = "ItemsSource";
        private static readonly string _CAROUSEL_ITEM_DATA_TEMPLATE_PROPERTY_PATH = "ItemDataTemplate";
        private static readonly string _CONTENT_HEIGH_PROPERTY_PATH = "ContentHeight";
        private static readonly string _CAROUSEL_POSITION_PROPERTY_PATH = "CarouselPosition";
        private static readonly string _IS_ANIMATE_TRANSITION_PROPERTY_PATH = "IsAnimateTransition";
        private static readonly string _PREV_BUTTON_IMAGE_SOURCE_PATH = "PeakMVP.Images.ic_left_arrow.png";
        private static readonly string _NEXT_BUTTON_IMAGE_SOURCE_PATH = "PeakMVP.Images.ic_right_arrow.png";

        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create("ItemsSource",
                typeof(IList),
                typeof(CompoundedCarousel),
                defaultValue: null);

        public static readonly BindableProperty ItemDataTemplateProperty =
            BindableProperty.Create("ItemDataTemplate",
                typeof(DataTemplate),
                typeof(CompoundedCarousel),
                defaultValue: null);

        public static readonly BindableProperty CarouselPositionProperty =
            BindableProperty.Create("CarouselPosition",
                typeof(int),
                typeof(CompoundedCarousel),
                defaultValue: 0);

        public static readonly BindableProperty IsContentVisibleProperty =
            BindableProperty.Create("IsContentVisible",
                typeof(bool),
                typeof(CompoundedCarousel),
                defaultValue: true,
                propertyChanged: OnIsContentVisiblePropertyChanged);

        public static readonly BindableProperty IsNavigationButtonsVisibleProperty =
            BindableProperty.Create("IsNavigationButtonsVisible",
                typeof(bool),
                typeof(CompoundedCarousel),
                defaultValue: true,
                propertyChanged: OnIsNavigationButtonsVisiblePropertyChanged);

        public static readonly BindableProperty IsAnimateTransitionProperty =
            BindableProperty.Create("IsAnimateTransition",
                typeof(bool),
                typeof(CompoundedCarousel),
                defaultValue: true);

        public static readonly BindableProperty ContentHeightProperty =
            BindableProperty.Create("ContentHeight",
                typeof(int),
                typeof(CompoundedCarousel),
                defaultValue: 1);

        private TapGestureRecognizer _prevButtontapGestureRecognizer;
        private TapGestureRecognizer _nextButtontapGestureRecognizer;

        private CarouselButtonBase _prevButton;
        private CarouselButtonBase _nextButton;

        public CompoundedCarousel() {
            InitializeComponent();

            _prevButtontapGestureRecognizer = new TapGestureRecognizer() {
                Command = new Command(OnPreviousTap)
            };
            _nextButtontapGestureRecognizer = new TapGestureRecognizer() {
                Command = new Command(OnNextTap)
            };

            ButtonsTemplate = new CircleButton();

            _carousel_CarouselView.SetBinding(CarouselView.FormsPlugin.Abstractions.CarouselViewControl.ItemsSourceProperty, new Binding(CompoundedCarousel._CAROUSEL_ITEM_SOURCE_PROPERTY_PATH, source: this));
            _carousel_CarouselView.SetBinding(CarouselView.FormsPlugin.Abstractions.CarouselViewControl.ItemTemplateProperty, new Binding(CompoundedCarousel._CAROUSEL_ITEM_DATA_TEMPLATE_PROPERTY_PATH, source: this));
            _carousel_CarouselView.SetBinding(CarouselView.FormsPlugin.Abstractions.CarouselViewControl.PositionProperty, new Binding(CompoundedCarousel._CAROUSEL_POSITION_PROPERTY_PATH, mode: BindingMode.TwoWay, source: this));
            _carousel_CarouselView.SetBinding(CarouselView.FormsPlugin.Abstractions.CarouselViewControl.AnimateTransitionProperty, new Binding(CompoundedCarousel._IS_ANIMATE_TRANSITION_PROPERTY_PATH, mode: BindingMode.TwoWay, source: this));

            _mainContent_AbsoluteLayout.SetBinding(View.HeightRequestProperty, new Binding(CompoundedCarousel._CONTENT_HEIGH_PROPERTY_PATH, source: this));

            OnIsContentVisible();
        }

        public IList ItemsSource {
            get => (IList)GetValue(CompoundedCarousel.ItemsSourceProperty);
            set => SetValue(CompoundedCarousel.ItemsSourceProperty, value);
        }

        public int CarouselPosition {
            get => (int)GetValue(CompoundedCarousel.CarouselPositionProperty);
            set => SetValue(CompoundedCarousel.CarouselPositionProperty, value);
        }

        public int ContentHeight {
            get => (int)GetValue(CompoundedCarousel.ContentHeightProperty);
            set => SetValue(CompoundedCarousel.ContentHeightProperty, value);
        }

        public bool IsAnimateTransition {
            get => (bool)GetValue(CompoundedCarousel.IsAnimateTransitionProperty);
            set => SetValue(CompoundedCarousel.IsAnimateTransitionProperty, value);
        }

        public bool IsContentVisible {
            get => (bool)GetValue(CompoundedCarousel.IsContentVisibleProperty);
            set => SetValue(CompoundedCarousel.IsContentVisibleProperty, value);
        }

        public bool IsNavigationButtonsVisible {
            get => (bool)GetValue(CompoundedCarousel.IsNavigationButtonsVisibleProperty);
            set => SetValue(CompoundedCarousel.IsNavigationButtonsVisibleProperty, value);
        }

        public DataTemplate ItemDataTemplate {
            get => (DataTemplate)GetValue(CompoundedCarousel.ItemDataTemplateProperty);
            set => SetValue(CompoundedCarousel.ItemDataTemplateProperty, value);
        }

        private CarouselButtonBase _buttonsTemplate;
        public CarouselButtonBase ButtonsTemplate {
            get => _buttonsTemplate;
            set {
                DisposeButtons();
                ApplyButtons(value);

                _buttonsTemplate = value;
            }
        }

        private static void OnIsContentVisiblePropertyChanged(BindableObject bindable, object oldValue, object newValue) {
            CompoundedCarousel compoundedCarousel = bindable as CompoundedCarousel;

            if (compoundedCarousel != null) {
                compoundedCarousel.OnIsContentVisible();
            }
        }

        private static void OnIsNavigationButtonsVisiblePropertyChanged(BindableObject bindable, object oldValue, object newValue) {
            CompoundedCarousel compoundedCarousel = bindable as CompoundedCarousel;

            if (compoundedCarousel != null) {
                compoundedCarousel.OnIsNavigationButtonsVisible();
            }
        }

        private void OnPreviousTap() {
            if ((_carousel_CarouselView.Position - 1) >= 0) {
                _carousel_CarouselView.Position--;
            }
        }

        private void OnNextTap() {
            if ((_carousel_CarouselView.Position + 1) <= _carousel_CarouselView.ItemsSource.Cast<object>().Count() - 1) {
                _carousel_CarouselView.Position++;
            }
        }

        private void OnIsContentVisible() {
            if (IsContentVisible) {
                Grid.SetRow(_mainContent_AbsoluteLayout, 1);
                OnIsNavigationButtonsVisible();
            }
            else {
                Grid.SetRow(_mainContent_AbsoluteLayout, 0);
                _prevButtonSpot_ContentView.IsVisible = false;
                _nextButtonSpot_ContentView.IsVisible = false;
            }
        }

        private void OnIsNavigationButtonsVisible() {
            _prevButtonSpot_ContentView.IsVisible = IsNavigationButtonsVisible;
            _nextButtonSpot_ContentView.IsVisible = IsNavigationButtonsVisible;
        }

        private void DisposeButtons() {
            if (_prevButton != null && _nextButton != null) {
                _prevButton.GestureRecognizers.Clear();
                _nextButton.GestureRecognizers.Clear();

                _prevButtonSpot_ContentView.Content = null;
                _nextButtonSpot_ContentView.Content = null;
            }
        }

        private void ApplyButtons(CarouselButtonBase buttonBackbone) {
            if (buttonBackbone != null) {
                DataTemplate buttonBuilderTemplate = new DataTemplate(buttonBackbone.GetType());

                _prevButton = (CarouselButtonBase)buttonBuilderTemplate.CreateContent();
                _nextButton = (CarouselButtonBase)buttonBuilderTemplate.CreateContent();

                _prevButton.GestureRecognizers.Add(_prevButtontapGestureRecognizer);
                _nextButton.GestureRecognizers.Add(_nextButtontapGestureRecognizer);

                _prevButton.Icon = ImageSource.FromResource(CompoundedCarousel._PREV_BUTTON_IMAGE_SOURCE_PATH);
                _nextButton.Icon = ImageSource.FromResource(CompoundedCarousel._NEXT_BUTTON_IMAGE_SOURCE_PATH);

                _prevButtonSpot_ContentView.Content = _prevButton;
                _nextButtonSpot_ContentView.Content = _nextButton;
            }
        }
    }
}