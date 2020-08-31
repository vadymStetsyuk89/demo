using Microsoft.AppCenter.Crashes;
using PeakMVP.Models.Identities.Medias;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace PeakMVP.Services.MediaPicker {
    public class MediaPickerService : IMediaPickerService {

        public static readonly string IMAGE_ATTACHED_MEDIA_DIRECTORY_NAME = "attached_image_media";
        public static readonly string VIDEO_ATTACHED_MEDIA_DIRECTORY_NAME = "attached_video_media";

        /// <summary>
        /// Takes photo with device camera.
        /// </summary>
        /// <returns></returns>
        public async Task<MediaFile> TakePhotoAsync() {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported) {
                return null;
            }

            MediaFile file = null;

            try {
                file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions {
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,
                    CompressionQuality = 70,
                    Name = string.Format("{0:ddMMyyHmmss}.jpg", DateTime.Now),
                    Directory = MediaPickerService.IMAGE_ATTACHED_MEDIA_DIRECTORY_NAME
                });
            }
            catch (Exception exc) {
                Crashes.TrackError(exc);

                Debugger.Break();
            }

            return file;
        }

        public async Task<MediaFile> PickPhotoAsync() {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickPhotoSupported) {
                return null;
            }
            var file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,
                CompressionQuality = 50
            });

            if (file == null)
                return null;

            return file;
        }

        /// <summary>
        /// Take video from device camera
        /// </summary>
        /// <returns></returns>
        public async Task<MediaFile> TakeVideoAsync() {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakeVideoSupported) {
                return null;
            }

            MediaFile file = null;

            try {
                file = await CrossMedia.Current.TakeVideoAsync(new StoreVideoOptions {
                    Quality = VideoQuality.Medium,
                    CompressionQuality = 1,
                    DesiredLength = new TimeSpan(0, 1, 0),
                    Directory = MediaPickerService.VIDEO_ATTACHED_MEDIA_DIRECTORY_NAME
                });
            }
            catch (Exception ex) {
                Crashes.TrackError(ex);

                Debugger.Break();
            }

            return file;
        }

        public async Task<MediaFile> PickVideoAsync() {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickVideoSupported) {
                return null;
            }

            var file = await CrossMedia.Current.PickVideoAsync();

            if (file == null)
                return null;

            return file;
        }

        public Task<PickedImage> BuildPickedImageAsync(MediaFile mediaFile) =>
            Task<PickedImage>.Run(async () => {
                PickedImage pickedImage = null;

                try {
                    pickedImage = new PickedImage();
                    pickedImage.DataBase64 = await ParseStreamToBase64(mediaFile.GetStream());
                    pickedImage.Name = Path.GetFileName(mediaFile.Path);
                }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    pickedImage = null;
                }

                return pickedImage;
            });

        public Task<string> ParseStreamToBase64(Stream stream) =>
            Task<string>.Run(() => {
                string base64string = "";

                try {
                    stream.Position = 0;

                    byte[] bytes;

                    using (BinaryReader reader = new BinaryReader(stream)) {
                        bytes = reader.ReadBytes((int)stream.Length);
                        base64string = Convert.ToBase64String(bytes);
                    }
                }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    base64string = "";
                }

                return base64string;
            });

        public Task<Stream> ExtractStreamFromMediaUrlAsync(string urlPath) =>
            Task<Stream>.Run(() => {
                Stream stream = null;

                try {
                    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(urlPath);
                    HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    stream = new MemoryStream();
                    httpWebResponse.GetResponseStream().CopyTo(stream);

                    httpWebResponse.Dispose();
                }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    stream = null;
                }

                return stream;
            });

    }
}
