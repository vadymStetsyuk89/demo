using System.Collections.Generic;
using System.Threading;

namespace PeakMVP.Services.Cancellation {
    public class CancellationService : ICancellationService {

        private List<CancellationTokenSource> _cancellationTokenSources;

        private CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        ///     ctor().
        /// </summary>
        public CancellationService() {
            _cancellationTokenSources = new List<CancellationTokenSource>();
        }

        public CancellationTokenSource GetCancellationTokenSource() {
            if (_cancellationTokenSource == null) {
                _cancellationTokenSource = new CancellationTokenSource();
            } else {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;

                _cancellationTokenSource = new CancellationTokenSource();
            }

            return _cancellationTokenSource;
        }

        public CancellationToken GetToken() {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            if (_cancellationTokenSources != null)
                _cancellationTokenSources.Add(cancellationTokenSource);

            return cancellationTokenSource.Token;
        }

        public void Cancel() {
            if (_cancellationTokenSources != null) {
                foreach (CancellationTokenSource cancellationTokenSource in _cancellationTokenSources) {
                    cancellationTokenSource.Cancel();
                    cancellationTokenSource.Dispose();
                }
                _cancellationTokenSources.Clear();
            }

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }
    }
}
