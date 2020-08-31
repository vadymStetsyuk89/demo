using AVFoundation;
using CoreGraphics;
using CoreMedia;
using Foundation;
using MobileCoreServices;
using PeakMVP.iOS.DependencyServices;
using PeakMVP.iOS.DependencyServices.Arguments;
using PeakMVP.Services.DependencyServices;
using PeakMVP.Services.DependencyServices.Arguments;
using PeakMVP.Services.MediaPicker;
using PeakMVP.Services.ProfileMedia;
using PeakMVP.ViewModels.Base;
using Plugin.Media.Abstractions;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(PickMediaDependencyService))]
namespace PeakMVP.iOS.DependencyServices {
    public class PickMediaDependencyService : IPickMediaDependencyService {

        private static readonly string _IMAGE_MEDIA_TYPE = "public.image";
        private static readonly string _MOVIE_MEDIA_TYPE = "public.movie";
        private static readonly string _TAKE_MEDIA_COMMON_ERROR = "Error occurred while taking media";
        private static readonly string _TAKE_IMAGE_COMMON_ERROR = "Error occurred while taking image";
        private static readonly string _TAKE_VIDEO_COMMON_ERROR = "Error occurred while taking video";

        public static readonly int IMAGE_WIDTH_RESTRICTION = 750;
        public static readonly int IMAGE_HEIGHT_RESTRICTION = 1334;

        private TaskCompletionSource<IPickedMediaFromGallery> _pickVideoTaskCompletion;
        private TaskCompletionSource<IPickedMediaFromGallery> _pickMediaTaskCompletion;
        private TaskCompletionSource<IPickedMediaFromGallery> _takeVideoOrImageTaskCompletion;

        public Task<IPickedMediaFromGallery> PickVideoAsync() {

            UIViewController topController = UIApplication.SharedApplication.KeyWindow.RootViewController;
            while (topController.PresentedViewController != null) {
                topController = topController.PresentedViewController;
            }

            _pickVideoTaskCompletion = new TaskCompletionSource<IPickedMediaFromGallery>();

            UIImagePickerController imagePicker = new UIImagePickerController();
            imagePicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
            imagePicker.MediaTypes = new string[] { UTType.Movie };

            imagePicker.FinishedPickingMedia += (sender, args) => {

                IPickedMediaFromGallery result = null;

                try {
                    string mediaTypeInfo = args.Info[UIImagePickerController.MediaType].ToString();

                    if (mediaTypeInfo == _MOVIE_MEDIA_TYPE) {
                        result = HandlePickedMovie(args);
                    }
                    else {
                        result = new PickedMediaFromGallery();
                    }
                }
                catch (Exception exc) {
                    result = new PickedMediaFromGallery() { Exception = exc };
                }

                topController.DismissModalViewController(true);

                _pickVideoTaskCompletion.SetResult(result);
            };

            imagePicker.Canceled += (sender, args) => {
                topController.DismissModalViewController(true);

                _pickVideoTaskCompletion.SetResult(new PickedMediaFromGallery() { Completion = true });
            };

            topController.PresentModalViewController(imagePicker, true);

            return _pickVideoTaskCompletion.Task;
        }

        public Task<IPickedMediaFromGallery> PickMediaAsync() {

            UIViewController topController = UIApplication.SharedApplication.KeyWindow.RootViewController;
            while (topController.PresentedViewController != null) {
                topController = topController.PresentedViewController;
            }

            _pickMediaTaskCompletion = new TaskCompletionSource<IPickedMediaFromGallery>();

            UIImagePickerController imagePicker = new UIImagePickerController();
            imagePicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
            imagePicker.MediaTypes = new string[] { UTType.Image, UTType.Movie };

            imagePicker.FinishedPickingMedia += (sender, args) => {

                string mediaTypeInfo = args.Info[UIImagePickerController.MediaType].ToString();
                IPickedMediaFromGallery result = null;

                if (mediaTypeInfo == _IMAGE_MEDIA_TYPE) {
                    result = HandlePickedImage(args);
                }
                else if (mediaTypeInfo == _MOVIE_MEDIA_TYPE) {
                    result = HandlePickedMovie(args);
                }
                else {
                    result = new PickedMediaFromGallery() {
                        Completion = false,
                        ErrorMessage = "iOS platform service exception"
                    };
                }

                topController.DismissModalViewController(true);

                _pickMediaTaskCompletion.SetResult(result);
            };

            imagePicker.Canceled += (sender, args) => {
                topController.DismissModalViewController(true);

                _pickMediaTaskCompletion.SetResult(new PickedMediaFromGallery() {
                    Completion = true
                });
            };

            topController.PresentModalViewController(imagePicker, true);

            return _pickMediaTaskCompletion.Task;
        }

        //public Task<IPickedMediaFromGallery> TakePhotoOrVideoAsync() =>
        //    Task<IPickedMediaFromGallery>.Run(() => {
        //        PickedMediaFromGallery pickedMediaResult = new PickedMediaFromGallery();

        //        try {
        //            UIViewController topController = UIApplication.SharedApplication.KeyWindow.RootViewController;
        //            while (topController.PresentedViewController != null) {
        //                topController = topController.PresentedViewController;
        //            }

        //            UIAlertController actionSheet = UIAlertController.Create("Make a choise", "Take Image or Video with:", UIAlertControllerStyle.ActionSheet);

        //            ///
        //            /// Use photo camera action
        //            /// 
        //            actionSheet.AddAction(UIAlertAction.Create("Camera", UIAlertActionStyle.Default, (action) => {
        //                if (UIImagePickerController.IsSourceTypeAvailable(UIImagePickerControllerSourceType.Camera)) {
        //                    UIImagePickerController imagePicker = new UIImagePickerController();
        //                    imagePicker.SourceType = UIImagePickerControllerSourceType.Camera;
        //                    imagePicker.MediaTypes = UIImagePickerController.AvailableMediaTypes(UIImagePickerControllerSourceType.Camera);

        //                    imagePicker.FinishedPickingMedia += (sender, args) => {
        //                        if (args?.OriginalImage != null) {
        //                            ///
        //                            /// Figure out how much to scale down by
        //                            /// 
        //                            int inSampleSize = GetInSampleSize(args.OriginalImage.Size.Width, args.OriginalImage.Size.Height);

        //                            UIImage originalImage = args.OriginalImage.Scale(new CGSize(args.OriginalImage.Size.Width / inSampleSize, args.OriginalImage.Size.Height / inSampleSize));
        //                            string imageBase64 = originalImage.AsJPEG().GetBase64EncodedString(NSDataBase64EncodingOptions.EndLineWithLineFeed);

        //                            pickedMediaResult.Completion = true;
        //                            pickedMediaResult.DataBase64 = imageBase64;
        //                            pickedMediaResult.DataThumbnailBase64 = imageBase64;
        //                            pickedMediaResult.MimeType = ProfileMediaService.IMAGE_MEDIA_TYPE;
        //                        }

        //                        topController.DismissModalViewController(true);
        //                    };

        //                    imagePicker.Canceled += (sender, args) => {
        //                        topController.DismissModalViewController(true);
        //                        pickedMediaResult.Completion = true;
        //                    };

        //                    topController.PresentModalViewController(imagePicker, true);
        //                }
        //                else {
        //                    UIAlertController alert = UIAlertController.Create("Warning", "Your device don't have camera", UIAlertControllerStyle.Alert);
        //                    alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, (alertAction) => {
        //                        pickedMediaResult.Completion = false;
        //                        pickedMediaResult.ErrorMessage = _TAKE_IMAGE_COMMON_ERROR;
        //                    }));

        //                    topController.PresentViewController(alert, true, null);
        //                }
        //            }));

        //            ///
        //            /// Use photo camera action
        //            /// 
        //            //actionSheet.AddAction(UIAlertAction.Create("Video camera", UIAlertActionStyle.Default, (action) => {
        //            //    if (UIVide.IsSourceTypeAvailable(UIImagePickerControllerSourceType.Camera)) {
        //            //        UIImagePickerController imagePicker = new UIImagePickerController();
        //            //        imagePicker.SourceType = UIImagePickerControllerSourceType.Camera;
        //            //        imagePicker.MediaTypes = UIImagePickerController.AvailableMediaTypes(UIImagePickerControllerSourceType.Camera);

        //            //        imagePicker.FinishedPickingMedia += (sender, args) => {
        //            //            if (args?.OriginalImage != null) {
        //            //                ///
        //            //                /// Figure out how much to scale down by
        //            //                /// 
        //            //                int inSampleSize = GetInSampleSize(args.OriginalImage.Size.Width, args.OriginalImage.Size.Height);

        //            //                UIImage originalImage = args.OriginalImage.Scale(new CGSize(args.OriginalImage.Size.Width / inSampleSize, args.OriginalImage.Size.Height / inSampleSize));
        //            //                string imageBase64 = originalImage.AsJPEG().GetBase64EncodedString(NSDataBase64EncodingOptions.EndLineWithLineFeed);

        //            //                pickedMediaResult.Completion = true;
        //            //                pickedMediaResult.DataBase64 = imageBase64;
        //            //                pickedMediaResult.DataThumbnailBase64 = imageBase64;
        //            //                pickedMediaResult.MimeType = ProfileMediaService.IMAGE_MEDIA_TYPE;
        //            //            }

        //            //            topController.DismissModalViewController(true);
        //            //        };

        //            //        imagePicker.Canceled += (sender, args) => {
        //            //            topController.DismissModalViewController(true);
        //            //            pickedMediaResult.Completion = true;
        //            //        };

        //            //        topController.PresentModalViewController(imagePicker, true);
        //            //    }
        //            //    else {
        //            //        UIAlertController alert = UIAlertController.Create("Warning", "Your device don't have camera", UIAlertControllerStyle.Alert);
        //            //        alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, (alertAction) => {
        //            //            pickedMediaResult.Completion = false;
        //            //            pickedMediaResult.ErrorMessage = _TAKE_IMAGE_COMMON_ERROR;
        //            //        }));

        //            //        topController.PresentViewController(alert, true, null);
        //            //    }
        //            //}));
        //        }
        //        catch (Exception exc) {
        //            pickedMediaResult.Completion = false;
        //            pickedMediaResult.Exception = exc;
        //            pickedMediaResult.ErrorMessage = _TAKE_MEDIA_COMMON_ERROR;
        //        }

        //        return (IPickedMediaFromGallery)pickedMediaResult;
        //    });

        public Task<IPickedMediaFromGallery> TakePhotoOrVideoAsync() {

            _takeVideoOrImageTaskCompletion = new TaskCompletionSource<IPickedMediaFromGallery>();

            try {
                UIViewController topController = UIApplication.SharedApplication.KeyWindow.RootViewController;
                while (topController.PresentedViewController != null) {
                    topController = topController.PresentedViewController;
                }

                IMediaPickerService mediaPickerService = ViewModelLocator.Resolve<IMediaPickerService>();

                UIAlertController actionSheet = UIAlertController.Create("Make a choise", "Take Image or Video with:", UIAlertControllerStyle.ActionSheet);

                ///
                /// Use photo camera action
                /// 
                actionSheet.AddAction(UIAlertAction.Create("Camera", UIAlertActionStyle.Default, async (action) => {
                    if (UIImagePickerController.IsSourceTypeAvailable(UIImagePickerControllerSourceType.Camera)) {
                        try {
                            MediaFile mediaFile = await mediaPickerService.TakePhotoAsync();

                            if (mediaFile != null) {
                                string base64 = await mediaPickerService.ParseStreamToBase64(mediaFile.GetStream());
                                _takeVideoOrImageTaskCompletion.SetResult(new PickedMediaFromGallery() { Completion = true, DataBase64 = base64, MimeType = ProfileMediaService.IMAGE_MEDIA_TYPE, DataThumbnailBase64 = base64 });
                                mediaFile.Dispose();
                            }
                            else {
                                _takeVideoOrImageTaskCompletion.SetResult(new PickedMediaFromGallery() { Completion = true });
                            }
                        }
                        catch (Exception exc) {
                            _takeVideoOrImageTaskCompletion.SetResult(new PickedMediaFromGallery() { ErrorMessage = _TAKE_IMAGE_COMMON_ERROR, Exception = exc });
                        }

                        topController.DismissModalViewController(true);
                    }
                    else {
                        UIAlertController alert = UIAlertController.Create("Warning", "Your device don't have camera", UIAlertControllerStyle.Alert);
                        alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, (alertAction) => _takeVideoOrImageTaskCompletion.SetResult(new PickedMediaFromGallery() { ErrorMessage = _TAKE_IMAGE_COMMON_ERROR })));

                        topController.PresentViewController(alert, true, null);
                    }
                }));

                ///
                /// Use photo camera action
                /// 
                actionSheet.AddAction(UIAlertAction.Create("Video camera", UIAlertActionStyle.Default, async (action) => {
                    if (UIImagePickerController.IsSourceTypeAvailable(UIImagePickerControllerSourceType.Camera)) {
                        try {
                            MediaFile mediaFile = await mediaPickerService.TakeVideoAsync();

                            if (mediaFile != null) {
                                string thumbnailBase64 = await mediaPickerService.ParseStreamToBase64(GetThumbnail(mediaFile.Path));
                                string base64 = await mediaPickerService.ParseStreamToBase64(mediaFile.GetStream());

                                _takeVideoOrImageTaskCompletion.SetResult(new PickedMediaFromGallery() { Completion = true, DataBase64 = base64, MimeType = ProfileMediaService.VIDEO_MEDIA_TYPE, DataThumbnailBase64 = thumbnailBase64 });

                                mediaFile.Dispose();
                            }
                            else {
                                _takeVideoOrImageTaskCompletion.SetResult(new PickedMediaFromGallery() { Completion = true });
                            }
                        }
                        catch (Exception exc) {
                            _takeVideoOrImageTaskCompletion.SetResult(new PickedMediaFromGallery() { ErrorMessage = _TAKE_IMAGE_COMMON_ERROR, Exception = exc });
                        }

                        topController.DismissModalViewController(true);
                    }
                    else {
                        UIAlertController alert = UIAlertController.Create("Warning", "Your device don't have camera", UIAlertControllerStyle.Alert);
                        alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, (alertAction) => _takeVideoOrImageTaskCompletion.SetResult(new PickedMediaFromGallery() { ErrorMessage = _TAKE_IMAGE_COMMON_ERROR })));

                        topController.PresentViewController(alert, true, null);
                    }
                }));

                topController.PresentViewController(actionSheet, true, null);
            }
            catch (Exception exc) {
                _takeVideoOrImageTaskCompletion.SetResult(new PickedMediaFromGallery() { Exception = exc, ErrorMessage = _TAKE_VIDEO_COMMON_ERROR });
            }

            return _takeVideoOrImageTaskCompletion.Task;
        }

        public Stream GetThumbnail(string pathMediaPath) {
            CMTime actualTime;
            NSError outError;

            try {
                using (var asset = AVAsset.FromUrl(NSUrl.FromFilename(pathMediaPath)))
                using (var imageGen = new AVAssetImageGenerator(asset)) {
                    imageGen.AppliesPreferredTrackTransform = true;
                    using (var imageRef = imageGen.CopyCGImageAtTime(new CMTime(1, 1), out actualTime, out outError)) {
                        return UIImage.FromImage(imageRef).AsPNG().AsStream();
                    }
                }
            }
            catch (Exception exc) {
                return null;
            }
        }

        private IPickedMediaFromGallery HandlePickedMovie(UIImagePickerMediaPickedEventArgs args) {
            NSData data = NSData.FromUrl(args.MediaUrl);
            string sourceBase64 = data.GetBase64EncodedString(NSDataBase64EncodingOptions.None);

            byte[] bytes;
            System.IO.Stream stream = GetThumbnail(args.MediaUrl.AbsoluteString);
            stream.Position = 0;

            using (System.IO.BinaryReader reader = new System.IO.BinaryReader(stream)) {
                bytes = reader.ReadBytes((int)stream.Length);
            }

            string sourceThumbnailBase64 = Convert.ToBase64String(bytes);

            return new PickedMediaFromGallery() {
                Completion = true,
                DataBase64 = sourceBase64,
                MimeType = ProfileMediaService.VIDEO_MEDIA_TYPE,
                DataThumbnailBase64 = sourceThumbnailBase64
            };
        }

        private IPickedMediaFromGallery HandlePickedImage(UIImagePickerMediaPickedEventArgs args) {
            int inSampleSize = GetInSampleSize(args.OriginalImage.Size.Width, args.OriginalImage.Size.Height);

            UIImage originalImage = args.OriginalImage.Scale(new CGSize(args.OriginalImage.Size.Width / inSampleSize, args.OriginalImage.Size.Height / inSampleSize));
            string imageBase64 = originalImage.AsJPEG().GetBase64EncodedString(NSDataBase64EncodingOptions.EndLineWithLineFeed);

            return new PickedMediaFromGallery() {
                Completion = true,
                MimeType = ProfileMediaService.IMAGE_MEDIA_TYPE,
                DataBase64 = imageBase64,
                DataThumbnailBase64 = imageBase64
            };
        }

        private int GetInSampleSize(nfloat srcWidth, nfloat srcHeight) {
            int inSampleSize = 1;

            if (srcHeight >= IMAGE_HEIGHT_RESTRICTION || srcWidth >= IMAGE_WIDTH_RESTRICTION) {
                if (srcHeight >= IMAGE_HEIGHT_RESTRICTION) {
                    inSampleSize = (int)Math.Round(srcHeight / IMAGE_HEIGHT_RESTRICTION);
                }
                else {
                    inSampleSize = (int)Math.Round(srcWidth / IMAGE_WIDTH_RESTRICTION);
                }
            }

            return inSampleSize;
        }
    }
}
