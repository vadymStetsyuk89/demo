using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PeakMVP.Views.CompoundedViews.MainContent.ProfileContent
{
    [XamlCompilation(XamlCompilationOptions.Skip)]
    public partial class PostContentView : ViewCell
    {

        public PostContentView()
        {
            InitializeComponent();
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            ForceUpdateSize();
        }

        private async void SendCommentTapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            ForceUpdateSize();

            /// Temporary crutch for iOS
            if (Device.RuntimePlatform == Device.iOS)
            {
                await Task.Delay(1030);
                ForceUpdateSize();
            }
        }
    }
}