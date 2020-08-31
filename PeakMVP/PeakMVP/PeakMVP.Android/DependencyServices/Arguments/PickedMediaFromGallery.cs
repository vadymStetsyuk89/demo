using PeakMVP.Services.DependencyServices.Arguments;
using System;

namespace PeakMVP.Droid.DependencyServices.Arguments {
    public class PickedMediaFromGallery : IPickedMediaFromGallery {

        public string DataBase64 { get; set; }

        public string DataThumbnailBase64 { get; set; }

        public string FilePath { get; set; }

        public bool Completion { get; set; }

        public Exception Exception { get; set; }

        public string ErrorMessage { get; set; }

        public string MimeType { get; set; }
    }
}