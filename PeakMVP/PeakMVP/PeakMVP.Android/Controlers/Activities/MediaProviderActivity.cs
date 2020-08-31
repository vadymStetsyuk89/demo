using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Database;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Util;
using PeakMVP.Droid.DependencyServices.Arguments;
using PeakMVP.Droid.Helpers;
using PeakMVP.Services.DependencyServices;
using PeakMVP.Services.DependencyServices.Arguments;
using PeakMVP.Services.ProfileMedia;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PeakMVP.Droid.Controlers.Activities {
    public abstract class MediaProviderActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity {

        private static readonly int _PICK_VIDEO_ASK_READ_EXTERNAL_STORAGE_PERMISSION_REQUEST_CODE = 2;
        private static readonly int _PICK_VIDEO_FROM_GALLERY_SELECT_GALLERY = 3;
        private static readonly int _PICK_VIDEO_FROM_GALLERYSELECT_GALLERY_KITKAT = 4;
        private static readonly int _TAKE_VIDEO_OR_IMAGE_PERMISSION_REQUEST_CODE = 5;

        private static readonly int _PICK_MEDIA_ASK_READ_EXTERNAL_STORAGE_PERMISSION_REQUEST_CODE = 5;
        private static readonly int _PICK_MEDIA_FROM_GALLERY_SELECT_GALLERY = 6;
        private static readonly int _PICK_MEDIA_FROM_GALLERY_SELECT_GALLERY_KITKAT = 7;

        private static readonly int _TAKE_IMAGE_OR_VIDEO_REQUEST_CODE = 8;

        private static readonly string _PEACK_MVP_DICTIONARY_NAME = "PeackMVP";
        private static readonly string _NO_PERMISSIONS_MEDIA_ERROR = "Haven't permissions";
        private static readonly string _EXTRACTION_MEDIA_MEDIA_PICK_ERROR = "Error occurred while extracting selected media";

        private static readonly string _TAKE_VIDEO_OR_IMAGE_PREPARE_INTENT_ERROR = "Error occurred while preparing intent to take video or image";
        private static readonly string _TAKE_VIDEO_OR_IMAGE_TAKED_IMAGE_HANDLING_ERROR = "Error occurred while handling taked image";
        private static readonly string _TAKE_VIDEO_OR_IMAGE_TAKED_VIDEO_HANDLING_ERROR = "Error occurred while handling taked video";

        public static TaskCompletionSource<IPickedMediaFromGallery> PickVideoTaskCompletion { get; set; }
        public static TaskCompletionSource<IPickedMediaFromGallery> PickMediaTaskCompletion { get; set; }
        public static TaskCompletionSource<IPickedMediaFromGallery> TakePhotoOrVideoCompletion { get; set; }

        private Java.IO.File _takeImageOutputFile;

        public void PickVideo() {
            PickVideoTaskCompletion = new TaskCompletionSource<IPickedMediaFromGallery>();
            CheckPermissionsForPickVideo();
        }

        public void PickMedia() {
            PickMediaTaskCompletion = new TaskCompletionSource<IPickedMediaFromGallery>();
            CheckPermissionsForPickMedia();
        }

        public void TakePhotoOrVideo() {
            TakePhotoOrVideoCompletion = new TaskCompletionSource<IPickedMediaFromGallery>();
            CheckPermissionsToTakeVideoOrImage();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults) {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (requestCode == _PICK_VIDEO_ASK_READ_EXTERNAL_STORAGE_PERMISSION_REQUEST_CODE) {
                if ((grantResults.Count() > 0) && (grantResults[0] == Permission.Granted)) {
                    SendIntentToPickVideo();
                }
                else {
                    PickVideoTaskCompletion.SetResult(new PickedMediaFromGallery() { ErrorMessage = _NO_PERMISSIONS_MEDIA_ERROR });
                }
            }
            else if (requestCode == _PICK_VIDEO_ASK_READ_EXTERNAL_STORAGE_PERMISSION_REQUEST_CODE) {
                if ((grantResults.Count() > 0) && (grantResults[0] == Permission.Granted)) {
                    SendIntentToPickMedia();
                }
                else {
                    PickMediaTaskCompletion.SetResult(new PickedMediaFromGallery() { ErrorMessage = _NO_PERMISSIONS_MEDIA_ERROR });
                }
            }
            else if (requestCode == _TAKE_VIDEO_OR_IMAGE_PERMISSION_REQUEST_CODE) {
                if (grantResults.Any() && grantResults.All(permission => permission == Permission.Granted)) {
                    SendIntentToTakeVideoOrImage();
                }
                else {
                    CompleteActionTakeVideoOrImage(new PickedMediaFromGallery() { ErrorMessage = _NO_PERMISSIONS_MEDIA_ERROR });
                }
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data) {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == _PICK_VIDEO_FROM_GALLERY_SELECT_GALLERY) {
                if (resultCode == Result.Ok) {
                    ExtractVideoPickedFromGallery(data);
                }
                else if (resultCode == Result.Canceled) {
                    PickVideoTaskCompletion.SetResult(new PickedMediaFromGallery() { Completion = true });
                }
            }
            else if (requestCode == _PICK_VIDEO_FROM_GALLERYSELECT_GALLERY_KITKAT) {
                if (resultCode == Result.Ok) {
                    ExtractVideoPickedFromGallery(data);
                }
                else if (resultCode == Result.Canceled) {
                    PickVideoTaskCompletion.SetResult(new PickedMediaFromGallery() { Completion = true });
                }
            }
            else if (requestCode == _PICK_MEDIA_FROM_GALLERY_SELECT_GALLERY) {
                if (resultCode == Result.Ok) {
                    ExtractMediaPickedFromGallery(data);
                }
                else if (resultCode == Result.Canceled) {
                    PickMediaTaskCompletion.SetResult(new PickedMediaFromGallery() { Completion = true });
                }
            }
            else if (requestCode == _PICK_MEDIA_FROM_GALLERY_SELECT_GALLERY_KITKAT) {
                if (resultCode == Result.Ok) {
                    ExtractMediaPickedFromGallery(data);
                }
                else if (resultCode == Result.Canceled) {
                    PickMediaTaskCompletion.SetResult(new PickedMediaFromGallery() { Completion = true });
                }
            }
            else if (requestCode == _TAKE_IMAGE_OR_VIDEO_REQUEST_CODE) {
                if (resultCode == Result.Ok) {
                    if (data == null) {
                        ExtractImageTakedFromCamera(data);
                    }
                    else {
                        ExtractVideoTakedFromCamera(data);
                    }
                }
                else {
                    CompleteActionTakeVideoOrImage(new PickedMediaFromGallery() { Completion = true });
                }
            }
        }

        private void CheckPermissionsForPickVideo() {
            Permission permissionCheck = ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage);

            if (permissionCheck != Permission.Granted) {
                ActivityCompat.RequestPermissions(
                    this,
                    new String[] { Manifest.Permission.ReadExternalStorage }, _PICK_VIDEO_ASK_READ_EXTERNAL_STORAGE_PERMISSION_REQUEST_CODE);
            }
            else {
                SendIntentToPickVideo();
            }
        }

        private void CheckPermissionsForPickMedia() {
            Permission permissionCheck = ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage);

            if (permissionCheck != Permission.Granted) {
                ActivityCompat.RequestPermissions(
                    this,
                    new String[] { Manifest.Permission.ReadExternalStorage }, _PICK_MEDIA_ASK_READ_EXTERNAL_STORAGE_PERMISSION_REQUEST_CODE);
            }
            else {
                SendIntentToPickMedia();
            }
        }

        private void CheckPermissionsToTakeVideoOrImage() {
            Permission writeCheck = ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage);
            Permission readCheck = ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage);
            Permission cameraCheck = ContextCompat.CheckSelfPermission(this, Manifest.Permission.Camera);

            if (writeCheck != Permission.Granted || readCheck != Permission.Granted || cameraCheck != Permission.Granted) {
                ActivityCompat.RequestPermissions(
                    this,
                    new String[] { Manifest.Permission.WriteExternalStorage, Manifest.Permission.ReadExternalStorage, Manifest.Permission.Camera }, _TAKE_VIDEO_OR_IMAGE_PERMISSION_REQUEST_CODE);
            }
            else {
                SendIntentToTakeVideoOrImage();
            }
        }

        private void SendIntentToTakeVideoOrImage() {
            try {
                StrictMode.VmPolicy.Builder builder = new StrictMode.VmPolicy.Builder();
                StrictMode.SetVmPolicy(builder.Build());

                _takeImageOutputFile = Java.IO.File.CreateTempFile(Guid.NewGuid().ToString(), ".jpeg", new Java.IO.File(GetExternalDirPath()));

                Intent takeImageIntent = new Intent(MediaStore.ActionImageCapture);
                takeImageIntent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(_takeImageOutputFile));

                Intent takeVideoIntent = new Intent(MediaStore.ActionVideoCapture);

                Intent chooserIntent = Intent.CreateChooser(takeImageIntent, "Take Image or Video with:");
                chooserIntent.PutExtra(Intent.ExtraInitialIntents, new Intent[] { takeVideoIntent });
                StartActivityForResult(chooserIntent, _TAKE_IMAGE_OR_VIDEO_REQUEST_CODE);
            }
            catch (Exception exc) {
                CompleteActionTakeVideoOrImage(new PickedMediaFromGallery() { Exception = exc, ErrorMessage = _TAKE_VIDEO_OR_IMAGE_PREPARE_INTENT_ERROR });
            }
        }

        private void SendIntentToPickVideo() {
            if ((int)Build.VERSION.SdkInt < 19) {
                Intent intent = new Intent(Intent.ActionGetContent);
                intent.SetType("video/*");
                StartActivityForResult(Intent.CreateChooser(intent, "Select with"), _PICK_VIDEO_FROM_GALLERY_SELECT_GALLERY);
            }
            else {
                Intent intent = new Intent(Intent.ActionOpenDocument);
                intent.SetType("video/*");
                StartActivityForResult(intent, _PICK_VIDEO_FROM_GALLERYSELECT_GALLERY_KITKAT);
                //StartActivityForResult(Intent.CreateChooser(intent, "Select with"), _PICK_VIDEO_FROM_GALLERYSELECT_GALLERY_KITKAT);
            }
        }

        private void SendIntentToPickMedia() {
            if ((int)Build.VERSION.SdkInt < 19) {
                Intent intent = new Intent(Intent.ActionGetContent);
                intent.SetType("image/* video/*");
                StartActivityForResult(Intent.CreateChooser(intent, "Select with"), _PICK_MEDIA_FROM_GALLERY_SELECT_GALLERY);
            }
            else {
                Intent intent = new Intent(Intent.ActionOpenDocument);
                intent.AddCategory(Intent.CategoryOpenable);
                intent.SetType("image/*");
                intent.PutExtra(Intent.ExtraMimeTypes, new String[] { "image/*", "video/*" });
                StartActivityForResult(Intent.CreateChooser(intent, "Select with"), _PICK_MEDIA_FROM_GALLERY_SELECT_GALLERY_KITKAT);
            }
        }

        private void ExtractVideoPickedFromGallery(Intent data) {
            try {
                Android.Net.Uri selectedUri = data.Data;

                string mimeType = "video";

                PickedMediaFromGallery result = new PickedMediaFromGallery();

                if (mimeType.StartsWith("video")) {
                    string filePath = MediaUtils.GetFileFullPathAlternativeVideo(data.Data, this);

                    Java.IO.File videoFile = new Java.IO.File(filePath);

                    Java.IO.FileInputStream videoFileInputStream = new Java.IO.FileInputStream(videoFile);
                    byte[] videoFileBytes = new byte[videoFile.Length()];
                    videoFileInputStream.Read(videoFileBytes);
                    videoFileInputStream.Close();
                    videoFileInputStream.Dispose();

                    System.IO.Stream thumbnailStream = DependencyService.Get<IPickMediaDependencyService>().GetThumbnail(filePath);
                    byte[] thumbnailBytes;

                    thumbnailStream.Position = 0;

                    using (System.IO.BinaryReader reader = new System.IO.BinaryReader(thumbnailStream)) {
                        thumbnailBytes = reader.ReadBytes((int)thumbnailStream.Length);
                    }

                    result = new PickedMediaFromGallery() {
                        Completion = true,
                        DataBase64 = Android.Util.Base64.EncodeToString(videoFileBytes, Android.Util.Base64Flags.Default),
                        DataThumbnailBase64 = Convert.ToBase64String(thumbnailBytes),
                        FilePath = filePath,
                        MimeType = ProfileMediaService.VIDEO_MEDIA_TYPE
                    };
                }
                else {
                    Debugger.Break();
                    throw new InvalidOperationException();
                }

                PickVideoTaskCompletion.SetResult(result);
            }
            catch (Exception exc) {
                Debugger.Break();

                PickVideoTaskCompletion.SetResult(new PickedMediaFromGallery() { Exception = exc, ErrorMessage = _EXTRACTION_MEDIA_MEDIA_PICK_ERROR });
            }
        }

        private void ExtractMediaPickedFromGallery(Intent data) {
            try {
                Android.Net.Uri selectedUri = data.Data;
                String[] columns = { MediaStore.Images.Media.InterfaceConsts.Data,
                         MediaStore.Images.Media.InterfaceConsts.MimeType };

                ICursor cursor = ContentResolver.Query(selectedUri, columns, null, null, null);
                cursor.MoveToFirst();

                int pathColumnIndex = cursor.GetColumnIndex(columns[0]);
                int mimeTypeColumnIndex = cursor.GetColumnIndex(columns[1]);

                string mimeType = cursor.GetString(mimeTypeColumnIndex);
                cursor.Close();

                PickedMediaFromGallery result = null;

                if (mimeType.StartsWith("image")) {
                    Android.Graphics.Bitmap bitmap = MediaUtils.GetScaledBitmap(
                        data.Data,
                        MediaUtils.IMAGE_WIDTH_RESTRICTION,
                        MediaUtils.IMAGE_HEIGHT_RESTRICTION,
                        this);

                    if (bitmap == null) {
                        bitmap = MediaStore.Images.Media.GetBitmap(ContentResolver, data.Data);
                    }

                    byte[] bytes;

                    using (MemoryStream memoryStream = new MemoryStream()) {
                        bitmap.Compress(Android.Graphics.Bitmap.CompressFormat.Jpeg, 100, memoryStream);
                        bytes = memoryStream.GetBuffer();
                    }

                    result = new PickedMediaFromGallery() {
                        DataBase64 = Android.Util.Base64.EncodeToString(bytes, Android.Util.Base64Flags.Default),
                        Completion = true,
                        DataThumbnailBase64 = Android.Util.Base64.EncodeToString(bytes, Android.Util.Base64Flags.Default),
                        MimeType = ProfileMediaService.IMAGE_MEDIA_TYPE
                    };
                }
                else if (mimeType.StartsWith("video")) {
                    string filePath = MediaUtils.GetFileFullPathAlternativeVideo(data.Data, this);

                    Java.IO.File videoFile = new Java.IO.File(filePath);

                    Java.IO.FileInputStream videoFileInputStream = new Java.IO.FileInputStream(videoFile);
                    byte[] videoFileBytes = new byte[videoFile.Length()];
                    videoFileInputStream.Read(videoFileBytes);
                    videoFileInputStream.Close();
                    videoFileInputStream.Dispose();

                    System.IO.Stream thumbnailStream = DependencyService.Get<IPickMediaDependencyService>().GetThumbnail(filePath);
                    byte[] thumbnailBytes;

                    thumbnailStream.Position = 0;

                    using (System.IO.BinaryReader reader = new System.IO.BinaryReader(thumbnailStream)) {
                        thumbnailBytes = reader.ReadBytes((int)thumbnailStream.Length);
                    }

                    result = new PickedMediaFromGallery() {
                        Completion = true,
                        DataBase64 = Android.Util.Base64.EncodeToString(videoFileBytes, Android.Util.Base64Flags.Default),
                        DataThumbnailBase64 = Convert.ToBase64String(thumbnailBytes),
                        FilePath = filePath,
                        MimeType = ProfileMediaService.VIDEO_MEDIA_TYPE
                    };

                }
                else {
                    Debugger.Break();
                    throw new InvalidOperationException();
                }

                PickMediaTaskCompletion.SetResult(result);
            }
            catch (Exception exc) {
                Debugger.Break();

                PickMediaTaskCompletion.SetResult(new PickedMediaFromGallery() { Exception = exc, ErrorMessage = _EXTRACTION_MEDIA_MEDIA_PICK_ERROR });
            }
        }

        private void ExtractImageTakedFromCamera(Intent data) {
            try {
                BitmapFactory.Options bmOptions = new BitmapFactory.Options();
                bmOptions.InJustDecodeBounds = true;
                BitmapFactory.DecodeFile(_takeImageOutputFile.AbsolutePath, bmOptions);
                int photoW = bmOptions.OutWidth;
                int photoH = bmOptions.OutHeight;

                //
                // Determine how much to scale down the image
                //
                int scaleFactor = Math.Min(photoW / MediaUtils.IMAGE_WIDTH_RESTRICTION, photoH / MediaUtils.IMAGE_HEIGHT_RESTRICTION);

                //
                // Decode the image file into a Bitmap sized to fill the View
                //
                bmOptions.InJustDecodeBounds = false;
                bmOptions.InSampleSize = scaleFactor;
                bmOptions.InPurgeable = true;

                Bitmap bitmap = BitmapFactory.DecodeFile(_takeImageOutputFile.AbsolutePath, bmOptions);

                byte[] bytes;

                using (MemoryStream memoryStream = new MemoryStream()) {
                    bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, memoryStream);
                    bytes = memoryStream.GetBuffer();
                }

                string base64String = Android.Util.Base64.EncodeToString(bytes, Base64Flags.Default);

                CompleteActionTakeVideoOrImage(new PickedMediaFromGallery() {
                    Completion = true,
                    DataBase64 = base64String,
                    DataThumbnailBase64 = base64String,
                    MimeType = ProfileMediaService.IMAGE_MEDIA_TYPE
                });
            }
            catch (Exception exc) {
                CompleteActionTakeVideoOrImage(new PickedMediaFromGallery() { Exception = exc, ErrorMessage = _TAKE_VIDEO_OR_IMAGE_TAKED_IMAGE_HANDLING_ERROR });
            }
        }

        private void ExtractVideoTakedFromCamera(Intent data) {
            try {
                Android.Net.Uri selectedUri = data.Data;

                string filePath = MediaUtils.GetFileFullPathAlternativeVideo(data.Data, this);

                Java.IO.File videoFile = new Java.IO.File(filePath);

                Java.IO.FileInputStream videoFileInputStream = new Java.IO.FileInputStream(videoFile);
                byte[] videoFileBytes = new byte[videoFile.Length()];
                videoFileInputStream.Read(videoFileBytes);
                videoFileInputStream.Close();
                videoFileInputStream.Dispose();

                System.IO.Stream thumbnailStream = DependencyService.Get<IPickMediaDependencyService>().GetThumbnail(filePath);
                byte[] thumbnailBytes;

                thumbnailStream.Position = 0;

                using (System.IO.BinaryReader reader = new System.IO.BinaryReader(thumbnailStream)) {
                    thumbnailBytes = reader.ReadBytes((int)thumbnailStream.Length);
                }

                CompleteActionTakeVideoOrImage(new PickedMediaFromGallery() {
                    Completion = true,
                    DataBase64 = Android.Util.Base64.EncodeToString(videoFileBytes, Android.Util.Base64Flags.Default),
                    DataThumbnailBase64 = Convert.ToBase64String(thumbnailBytes),
                    FilePath = filePath,
                    MimeType = ProfileMediaService.VIDEO_MEDIA_TYPE
                });
            }
            catch (Exception exc) {
                CompleteActionTakeVideoOrImage(new PickedMediaFromGallery() { Exception = exc, ErrorMessage = _TAKE_VIDEO_OR_IMAGE_TAKED_VIDEO_HANDLING_ERROR });
            }
        }

        private void CompleteActionTakeVideoOrImage(IPickedMediaFromGallery completionResult) {
            try {
                _takeImageOutputFile.Delete();
            }
            catch (Exception exc) {
                Debugger.Break();
            }

            TakePhotoOrVideoCompletion.SetResult(completionResult);
        }

        private bool ResolveTakedVideoDuration(string videoPath, int durationLimitSeconds) {
            Android.Media.MediaPlayer mediaPlayer = Android.Media.MediaPlayer.Create(this, Android.Net.Uri.Parse(videoPath));

            bool result = false;

            if ((mediaPlayer.Duration / 1000) < durationLimitSeconds) {
                result = true;
            }

            return result;
        }

        private bool ResolveTakedFileSize(Java.IO.File targetFile, int sizeLimitInKiloBytes) {
            bool result = false;

            long length = targetFile.Length();

            result = ((length / 1024) < sizeLimitInKiloBytes);

            return result;
        }

        private string GetExternalDirPath() {
            string dirPath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            string targetDir = System.IO.Path.Combine(dirPath, _PEACK_MVP_DICTIONARY_NAME);

            bool isDirExist = System.IO.Directory.Exists(targetDir);

            if (isDirExist) {
                return targetDir;
            }
            else {
                System.IO.Directory.CreateDirectory(targetDir);
            }

            return targetDir;
        }
    }
}