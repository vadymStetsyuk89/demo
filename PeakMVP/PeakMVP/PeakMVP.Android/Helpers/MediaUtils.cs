using Android.Content;
using Android.Database;
using Android.Graphics;
using System;

namespace PeakMVP.Droid.Helpers {
    public class MediaUtils {

        //public static readonly int IMAGE_WIDTH_RESTRICTION = 360;
        //public static readonly int IMAGE_HEIGHT_RESTRICTION = 640;
        public static readonly int IMAGE_WIDTH_RESTRICTION = 750;
        public static readonly int IMAGE_HEIGHT_RESTRICTION = 1334;

        /// <summary>
        /// Allow to get full path to the data on all android devices
        /// </summary>
        public static string GetFileFullPathAlternativeVideo(Android.Net.Uri uri, Context context) {
            if (uri.Scheme == "file") {
                return uri.Path;
            }
            else {
                string doc_id = "";

                using (var c1 = context.ContentResolver.Query(uri, null, null, null, null)) {
                    c1.MoveToFirst();
                    String document_id = c1.GetString(0);
                    doc_id = document_id.Substring(document_id.LastIndexOf(":") + 1);
                }

                string path = "";

                string selection = Android.Provider.MediaStore.Video.Media.InterfaceConsts.Id + " =? ";
                using (ICursor cursor = context.ContentResolver.Query(Android.Provider.MediaStore.Video.Media.ExternalContentUri, null, selection, new string[] { doc_id }, null)) {
                    if (cursor == null)
                        return path;
                    var columnIndex = cursor.GetColumnIndexOrThrow(Android.Provider.MediaStore.Video.Media.InterfaceConsts.Data);
                    cursor.MoveToFirst();
                    path = cursor.GetString(columnIndex);
                }

                return path;
            }
        }

        public static Bitmap GetScaledBitmap(Android.Net.Uri uri, int destWidth, int destHeight, Context context) {
            string fullPath = GetFileFullPathAlternative(uri, context);

            // Read in the dimensions of the image on disk
            BitmapFactory.Options options = new BitmapFactory.Options();
            options.InJustDecodeBounds = true;
            BitmapFactory.DecodeFile(fullPath, options);

            float srcWidth = options.OutWidth;
            float srcHeight = options.OutHeight;

            // Figure out how much to scale down by
            int inSampleSize = 1;

            if (srcHeight >= destHeight || srcWidth >= destWidth) {
                if (srcHeight >= destHeight) {
                    inSampleSize = (int)Math.Round(srcHeight / destHeight);
                }
                else {
                    inSampleSize = (int)Math.Round(srcWidth / destWidth);
                }
            }

            options = new BitmapFactory.Options();
            options.InSampleSize = inSampleSize;

            // Read in and create final bitmap
            return BitmapFactory.DecodeFile(fullPath, options);
        }

        public static string GetFileFullPathAlternative(Android.Net.Uri uri, Context context) {
            string doc_id = "";

            using (var c1 = context.ContentResolver.Query(uri, null, null, null, null)) {
                c1.MoveToFirst();
                String document_id = c1.GetString(0);
                doc_id = document_id.Substring(document_id.LastIndexOf(":") + 1);
            }

            string path = "";

            string selection = Android.Provider.MediaStore.Images.Media.InterfaceConsts.Id + " =? ";
            using (ICursor cursor = context.ContentResolver.Query(Android.Provider.MediaStore.Images.Media.ExternalContentUri, null, selection, new string[] { doc_id }, null)) {
                if (cursor == null) return path;
                var columnIndex = cursor.GetColumnIndexOrThrow(Android.Provider.MediaStore.Images.Media.InterfaceConsts.Data);
                cursor.MoveToFirst();
                path = cursor.GetString(columnIndex);
            }

            return path;
        }
    }
}