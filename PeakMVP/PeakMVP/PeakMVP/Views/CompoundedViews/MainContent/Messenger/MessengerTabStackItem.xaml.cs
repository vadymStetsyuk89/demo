using FFImageLoading.Transformations;
using PeakMVP.Controls.Stacklist;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PeakMVP.Views.CompoundedViews.MainContent.Messenger {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MessengerTabStackItem : CommonStackListItem {

        private static readonly string _BLUE_COLOR_APP_RESSOURCE_KEY = "BlueColor";
        private static readonly string _WHITE_COLOR_APP_RESSOURCE_KEY = "WhiteColor";

        private readonly ColorSpaceTransformation _colorSpaceTransformation = new ColorSpaceTransformation();

        public MessengerTabStackItem() {
            InitializeComponent();
        }

        public override void Deselected() {
            if (IsOnSelectionVisualChangesEnabled) {
                _roundedContentBox_ExtendedContentView.BackgroundColor = (Color)Application.Current.Resources[_WHITE_COLOR_APP_RESSOURCE_KEY];
                _header_Label.TextColor = (Color)Application.Current.Resources[_BLUE_COLOR_APP_RESSOURCE_KEY];
                _messagesCounter_Label.TextColor = (Color)Application.Current.Resources[_BLUE_COLOR_APP_RESSOURCE_KEY];

                Grid.SetColumn(_blueIcon_CachedImage, 1);
                Grid.SetColumn(_whiteIcon_CachedImage, 0);
            }
        }

        public override void Selected() {
            if (IsOnSelectionVisualChangesEnabled) {
                _roundedContentBox_ExtendedContentView.BackgroundColor = (Color)Application.Current.Resources[_BLUE_COLOR_APP_RESSOURCE_KEY];
                _header_Label.TextColor = (Color)Application.Current.Resources[_WHITE_COLOR_APP_RESSOURCE_KEY];
                _messagesCounter_Label.TextColor = (Color)Application.Current.Resources[_WHITE_COLOR_APP_RESSOURCE_KEY];

                Grid.SetColumn(_blueIcon_CachedImage, 0);
                Grid.SetColumn(_whiteIcon_CachedImage, 1);
            }
        }
    }
}