using PeakMVP.ViewModels.MainContent.ProfileContent;
using System;

namespace PeakMVP.Models.Arguments.AppEventsArguments.Posts {
    public abstract class PostManagingBaseArgs : EventArgs {

        public PostContentViewModel TargetPost { get; set; }
    }
}
