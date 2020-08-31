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
using Com.Google.Android.Exoplayer2.Upstream;
using Com.Google.Android.Exoplayer2.Upstream.Cache;
using Com.Google.Android.Exoplayer2.Util;

namespace PeakMVP.Droid.Renderers.CrossVideoPlayer {
    public class CacheableDataSource : Java.Lang.Object, IDataSourceFactory {

        private readonly Context _context;
        private readonly DefaultDataSourceFactory _defaultDatasourceFactory;
        private readonly long _maxFileSize;
        private readonly long _maxCacheSize;

        public CacheableDataSource(Context context, long maxCacheSize, long maxFileSize) {
            _context = context;
            _maxCacheSize = maxCacheSize;
            _maxFileSize = maxFileSize;

            string userAgent = Util.GetUserAgent(context, _context.GetString(Resource.String.application_name));

            DefaultBandwidthMeter bandwidthMeter = new DefaultBandwidthMeter();

            _defaultDatasourceFactory =
                new DefaultDataSourceFactory(
                    this._context,
                    bandwidthMeter,
                    new DefaultHttpDataSourceFactory(userAgent, bandwidthMeter));
        }

        public IDataSource CreateDataSource() {
            LeastRecentlyUsedCacheEvictor evictor = new LeastRecentlyUsedCacheEvictor(_maxCacheSize);
            SimpleCache simpleCache = new SimpleCache(new Java.IO.File(_context.CacheDir, "media"), evictor);

            return new CacheDataSource(
                simpleCache,
                _defaultDatasourceFactory.CreateDataSource(),
                new FileDataSource(),
                new CacheDataSink(simpleCache, _maxFileSize),
                CacheDataSource.FlagBlockOnCache | CacheDataSource.FlagIgnoreCacheOnError,
                null);
        }
    }
}