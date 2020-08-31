using System;

namespace PeakMVP.Services.DependencyServices.Arguments {
    public interface IPickedMediaFromGallery {

        string DataBase64 { get; set; }

        string DataThumbnailBase64 { get; set; }

        string FilePath { get; set; }

        bool Completion { get; set; }

        Exception Exception { get; set; }

        string ErrorMessage { get; set; }

        string MimeType { get; set; }
    }
}
