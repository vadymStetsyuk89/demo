using PeakMVP.Controls.Stacklist;
using System.Collections;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PeakMVP.Views.CompoundedViews.MainContent.Events {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EventsAndGamesGridContentView : ContentView {

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
            nameof(ItemsSource),
            typeof(IList),
            typeof(EventsAndGamesGridContentView));

        public static readonly BindableProperty IsManagementAvailableProperty = BindableProperty.Create(
            nameof(IsManagementAvailable),
            typeof(bool),
            typeof(EventsAndGamesGridContentView),
            defaultValue: default(bool),
            propertyChanged: (BindableObject bindable, object oldValue, object newValue) => {
                if (bindable is EventsAndGamesGridContentView declarer) {

                }
            });

        public EventsAndGamesGridContentView() {
            InitializeComponent();

            _gamesEvensListValues_StackList.SetBinding(StackList.ItemsSourceProperty, new Binding(nameof(ItemsSource), source: this));

            //_managingTitleSeparator_BoxView.SetBinding(VisualElement.IsVisibleProperty, new Binding(nameof(IsManagementAvailable), source: this));
            //_managingTitle_Label.SetBinding(VisualElement.IsVisibleProperty, new Binding(nameof(IsManagementAvailable), source: this));
        }

        public IList ItemsSource {
            get => (IList)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public bool IsManagementAvailable {
            get => (bool)GetValue(IsManagementAvailableProperty);
            set => SetValue(IsManagementAvailableProperty, value);
        }
    }
}