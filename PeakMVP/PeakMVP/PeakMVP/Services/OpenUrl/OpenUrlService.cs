using System;
using Xamarin.Forms;

namespace PeakMVP.Services.OpenUrl {
    public class OpenUrlService : IOpenUrlService {
        public void OpenUrl(string url) {
            Device.OpenUri(new Uri(url));
        }
    }
}
