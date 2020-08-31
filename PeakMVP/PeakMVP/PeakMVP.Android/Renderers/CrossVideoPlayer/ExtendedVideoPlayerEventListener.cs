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
using Com.Google.Android.Exoplayer2.Source;
using Com.Google.Android.Exoplayer2.Trackselection;
using Java.Lang;

namespace PeakMVP.Droid.Renderers.CrossVideoPlayer {
    public class ExtendedVideoPlayerEventListener : Java.Lang.Object, IPlayerEventListener {

        private SimpleExoPlayer _exoPlayer;

        public ExtendedVideoPlayerEventListener(SimpleExoPlayer targetPlayer) {
            _exoPlayer = targetPlayer;
        }

        public void OnLoadingChanged(bool p0) { }

        public void OnPlaybackParametersChanged(PlaybackParameters p0) { }

        public void OnPlayerError(ExoPlaybackException p0) { }

        public void OnPlayerStateChanged(bool p0, int p1) {
            //
            // If video completely played
            //
            if (p1 == 4) {
                _exoPlayer.SeekTo(0);
                _exoPlayer.PlayWhenReady = false;
            }
        }

        public void OnPositionDiscontinuity(int p0) { }

        public void OnRepeatModeChanged(int p0) { }

        public void OnSeekProcessed() { }

        public void OnShuffleModeEnabledChanged(bool p0) { }

        public void OnTimelineChanged(Timeline p0, Java.Lang.Object p1, int p2) { }

        public void OnTracksChanged(TrackGroupArray p0, TrackSelectionArray p1) { }
    }
}