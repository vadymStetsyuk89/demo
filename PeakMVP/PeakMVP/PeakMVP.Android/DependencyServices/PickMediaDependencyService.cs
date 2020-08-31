using Android.Graphics;
using Android.Media;
using Android.Provider;
using PeakMVP.Droid.Controlers.Activities;
using PeakMVP.Droid.DependencyServices;
using PeakMVP.Services.DependencyServices;
using PeakMVP.Services.DependencyServices.Arguments;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(PickMediaDependencyService))]
namespace PeakMVP.Droid.DependencyServices {
    public class PickMediaDependencyService : IPickMediaDependencyService {

        public System.IO.Stream GetThumbnail(string pathMediaPath) {
            try {
                Bitmap thumb = ThumbnailUtils.CreateVideoThumbnail(pathMediaPath, ThumbnailKind.FullScreenKind);

                System.IO.Stream stream = new MemoryStream();
                thumb.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
                stream.Seek(0L, SeekOrigin.Begin);

                return stream;
            }
            catch (Exception exc) {
                Debugger.Break();

                return null;
            }
        }

        public Task<IPickedMediaFromGallery> PickVideoAsync() {
            ((MediaProviderActivity)MainActivity.Self).PickVideo();

            return MediaProviderActivity.PickVideoTaskCompletion.Task;
        }

        public Task<IPickedMediaFromGallery> PickMediaAsync() {
            ((MediaProviderActivity)MainActivity.Self).PickMedia();

            return MediaProviderActivity.PickMediaTaskCompletion.Task;
        }

        public Task<IPickedMediaFromGallery> TakePhotoOrVideoAsync() {
            ((MediaProviderActivity)MainActivity.Self).TakePhotoOrVideo();

            return MediaProviderActivity.TakePhotoOrVideoCompletion.Task;
        }
    }
}