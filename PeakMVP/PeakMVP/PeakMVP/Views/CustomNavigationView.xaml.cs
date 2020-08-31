using PeakMVP.ViewModels.Base;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PeakMVP.Views {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomNavigationView : NavigationPage {

        public static readonly string ON_CUSTOM_NAVIGATION_VIEW_APPEARING = "on_custom_navigation_view_appearing";

        public CustomNavigationView() : base() {
            InitializeComponent();
        }

        public CustomNavigationView(Page root) : base(root) {
            InitializeComponent();
        }

        protected async override void OnAppearing() {
            base.OnAppearing();

            //await Task.Delay(12000);
            //MessagingCenter.Send<object>(this, ON_CUSTOM_NAVIGATION_VIEW_APPEARING);
        }

        protected override bool OnBackButtonPressed() {
            Page lastPageInTheStack = Pages.LastOrDefault<Page>();

            //
            // If navigation stack contains only one page - dispose and pop that page (without navigation service)
            //
            if (Pages.Count() <= 1) {
                if (lastPageInTheStack.BindingContext != null && lastPageInTheStack.BindingContext is ViewModelBase viewModel) {
                    viewModel.Dispose();
                }
                return base.OnBackButtonPressed();
            }

            if (lastPageInTheStack != null &&
                lastPageInTheStack.BindingContext != null &&
                lastPageInTheStack.BindingContext is ViewModelBase &&
                (lastPageInTheStack.BindingContext as ViewModelBase).BackCommand != null) {
                ((ViewModelBase)lastPageInTheStack.BindingContext).BackCommand.Execute(null);

                return true;
            }
            else {
                return base.OnBackButtonPressed();
            }
        }
    }
}