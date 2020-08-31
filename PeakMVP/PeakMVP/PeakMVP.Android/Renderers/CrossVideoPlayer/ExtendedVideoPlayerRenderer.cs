using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Trackselection;
using Com.Google.Android.Exoplayer2.UI;
using Com.Google.Android.Exoplayer2.Upstream;
using PeakMVP.Droid.Renderers.CrossVideoPlayer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Com.Google.Android.Exoplayer2.Util;
using Com.Google.Android.Exoplayer2.Source;
using Com.Google.Android.Exoplayer2.Extractor;
using System.ComponentModel;
using PeakMVP.Controls.CrossVideoPlayer;

[assembly: ExportRenderer(typeof(ExtendedVideoPlayer), typeof(ExtendedVideoPlayerRenderer))]
namespace PeakMVP.Droid.Renderers.CrossVideoPlayer {
    public class ExtendedVideoPlayerRenderer : ViewRenderer<ExtendedVideoPlayer, PlayerView> {

        private SimpleExoPlayer _exoPlayer;
        private PlayerView _exoSpot;

        public ExtendedVideoPlayerRenderer(Context context) :
            base(context) { }

        protected override void OnElementChanged(ElementChangedEventArgs<ExtendedVideoPlayer> e) {
            base.OnElementChanged(e);

            string s = Element.Source;

            //
            // Will be invoked by the Xamarin.Forms page when the last is closed
            //
            Element.ReleaseAction = PlayerRelease;

            _exoSpot = new PlayerView(Context);
            //
            // Video will be expanded on full screen
            //
            _exoSpot.SetResizeMode(AspectRatioFrameLayout.ResizeModeFit);

            SetNativeControl(_exoSpot);

            //
            // Create a default TrackSelector
            //
            Handler mainHandler = new Handler();
            IBandwidthMeter bandwidthMeter = new DefaultBandwidthMeter();

            AdaptiveTrackSelection.Factory videoTrackSelectionFactory =
                new AdaptiveTrackSelection.Factory(bandwidthMeter);

            TrackSelector trackSelector =
                new DefaultTrackSelector(videoTrackSelectionFactory);

            //
            // Create the player
            //
            _exoPlayer =
                ExoPlayerFactory.NewSimpleInstance(Context, trackSelector);
            _exoPlayer.AddListener(new ExtendedVideoPlayerEventListener(_exoPlayer));
            _exoPlayer.PlayWhenReady = true;

            //
            // Attach player to the view
            //
            _exoSpot.Player = _exoPlayer;

            DefaultDataSourceFactory dataSourceFactory = new DefaultDataSourceFactory(Context,
                Util.GetUserAgent(Context, Context.GetString(Resource.String.application_name)));

            IMediaSource videoSource = new ExtractorMediaSource(
                Android.Net.Uri.Parse(Element.Source),
                new CacheableDataSource(Context, 100 * 1024 * 1024, 5 * 1024 * 1024),
                new DefaultExtractorsFactory(),
                null,
                null);

            //
            // Prepare the player with the source.
            //
            _exoPlayer.Prepare(videoSource);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e) {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == ExtendedVideoPlayer.IsAppearedProperty.PropertyName) {
                if (!(Element.IsAppeared) &&
                    _exoPlayer != null) {
                    _exoPlayer.PlayWhenReady = false;
                }
            }
            else if (e.PropertyName == ExtendedVideoPlayer.SourceProperty.PropertyName) {
                string s = Element.Source;
                _exoPlayer.Stop();
                IMediaSource videoSource = new ExtractorMediaSource(
                    Android.Net.Uri.Parse(Element.Source),
                    new CacheableDataSource(Context, 100 * 1024 * 1024, 5 * 1024 * 1024),
                    new DefaultExtractorsFactory(),
                    null,
                    null);
                _exoPlayer.Prepare(videoSource);
            }
        }

        private void PlayerRelease() {
            if (_exoPlayer != null) {
                _exoPlayer.PlayWhenReady = false;
                _exoPlayer.Release();
            }
        }
    }
}