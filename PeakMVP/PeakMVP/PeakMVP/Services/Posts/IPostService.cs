using PeakMVP.Models.Identities.Feed;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Requests.RequestDataModels.Posts;
using PeakMVP.Models.Rests.Responses.Posts;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PeakMVP.Services.Posts {
    public interface IPostService {
        Task<List<PostDTO>> GetPostsAsync(string authorshortId = "", string pageId = "", CancellationToken cancellationToken = default(CancellationToken));

        Task<bool> PublishPostAsync(PublishPostDataModel publishPostDataModel, CancellationTokenSource cancellationTokenSource);

        Task<CommentDTO> PublishCommentAsync(PublishCommentDataModel publishCommentDataModel, CancellationToken token);

        Task<bool> DeletePostByPostId(long postId, CancellationTokenSource cancellationTokenSource);

        Task<List<PostPublicityScope>> GetPossiblePostPublicityScopesAsync(CancellationTokenSource cancellationTokenSource);

        Task<bool> EditPostAsync(long postId, PublishPostDataModel publishPostDataModel, CancellationTokenSource cancellationTokenSource);
    }
}
