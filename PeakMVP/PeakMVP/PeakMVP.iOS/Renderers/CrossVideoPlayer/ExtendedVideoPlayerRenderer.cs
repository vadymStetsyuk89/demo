using AVKit;
using CoreGraphics;
using Foundation;
using PeakMVP.Controls.CrossVideoPlayer;
using PeakMVP.iOS.Renderers.CrossVideoPlayer;
using System.ComponentModel;
using UIKit;
using WebKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ExtendedVideoPlayer), typeof(ExtendedVideoPlayerRenderer))]
namespace PeakMVP.iOS.Renderers.CrossVideoPlayer {
    public class ExtendedVideoPlayerRenderer : ViewRenderer<ExtendedVideoPlayer, UIView> {

        private AVPictureInPictureController _avPlayer;

        private AVPlayerViewController _avXXXPlayer;

        private WKWebView _wkWebView;

        protected override void OnElementChanged(ElementChangedEventArgs<ExtendedVideoPlayer> e) {
            base.OnElementChanged(e);

            if (e.NewElement != null) {
                if (Control == null) {
                    _wkWebView = new WKWebView(new CGRect(), new WKWebViewConfiguration());

                    SetNativeControl(_wkWebView);
                }
            }

            if (Element == null)
                return;
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e) {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == ExtendedVideoPlayer.SourceProperty.PropertyName) {
                UpdateVideoPath();
            }
            else if (e.PropertyName == ExtendedVideoPlayer.IsAppearedProperty.PropertyName) {
                try {
                    if (Element.IsAppeared) {
                        
                    }
                }
                catch {
                    System.Diagnostics.Debugger.Break();
                }
            }
        }

        private void UpdateVideoPath() {
            if (_wkWebView != null && Element != null) {
                var url = new NSUrl(Element.Source);
                var request = new NSUrlRequest(url);
                _wkWebView.LoadRequest(request);
            }
        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);

            if (disposing) {
                if (_wkWebView != null) {
                    _wkWebView.Dispose();
                    _wkWebView = null;
                }
            }
        }
    }
}
