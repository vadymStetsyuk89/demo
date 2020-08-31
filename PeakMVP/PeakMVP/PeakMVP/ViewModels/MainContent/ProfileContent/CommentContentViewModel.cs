using PeakMVP.ViewModels.Base;
using System;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.MainContent.ProfileContent {
    public sealed class CommentContentViewModel : ViewModelBase {

        string _avatar;
        public string Avatar {
            get { return _avatar; }
            set { SetProperty(ref _avatar, value); }
        }

        string _displayName;
        public string DisplayName {
            get { return _displayName; }
            set { SetProperty(ref _displayName, value); }
        }

        string _text;
        public string Text {
            get { return _text; }
            set { SetProperty(ref _text, value); }
        }

        FormattedString _formatedComment;
        public FormattedString FormatedComment {
            get { return _formatedComment; }
            set { SetProperty(ref _formatedComment, value); }
        }

        DateTime _creationTime;
        public DateTime CreationTime {
            get { return _creationTime; }
            set { SetProperty(ref _creationTime, value); }
        }

        /// <summary>
        ///     ctor().
        /// </summary>
        public CommentContentViewModel() {

        }
    }
}
