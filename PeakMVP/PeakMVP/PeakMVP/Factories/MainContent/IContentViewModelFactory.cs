using PeakMVP.Models.Identities;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.ViewModels.MainContent.ProfileContent;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PeakMVP.Factories.MainContent {
    public interface IContentViewModelFactory {
        List<PostContentViewModel> CreatePostContentViewModels(IEnumerable<PostDTO> foundedPosts);

        ObservableCollection<CommentContentViewModel> CreateComments(IEnumerable<CommentDTO> source);

        CommentContentViewModel CreateComment(CommentDTO source, UserProfile author);
    }
}
