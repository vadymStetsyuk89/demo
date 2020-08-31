using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Requests.RequestDataModels.Posts;
using System;

namespace PeakMVP.Models.Arguments.InitializeArguments.Post {
    public class AttachExternalMediaToNewPostArgs : EventArgs {
        public AttachedFileDataModel NewMedia { get; set; }

        public MediaDTO MediaDTO { get; set; }
    }
}
