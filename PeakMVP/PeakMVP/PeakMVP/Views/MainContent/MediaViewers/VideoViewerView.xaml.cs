using PeakMVP.Views.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PeakMVP.Views.MainContent.MediaViewers {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VideoViewerView : ContentPageBase {

        public VideoViewerView() {
            InitializeComponent();
        }

        protected override void OnAppearing() {
            base.OnAppearing();

            _videoPlayer_ExtendedVideoPlayer.IsAppeared = true;
        }

        protected override void OnDisappearing() {
            base.OnDisappearing();

            _videoPlayer_ExtendedVideoPlayer.IsAppeared = false;
        }
    }
}