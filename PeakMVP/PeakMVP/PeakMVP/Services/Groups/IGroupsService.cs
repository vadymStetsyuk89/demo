using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Requests.RequestDataModels.Groups;
using PeakMVP.Models.Rests.Responses.Groups;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PeakMVP.Services.Groups {
    public interface IGroupsService {
        Task<List<GroupDTO>> GetGroupsAsync(CancellationTokenSource cancellationTokenSource);

        Task<bool> DeleteGroupByIdAsync(long groupId, CancellationTokenSource cancellationTokenSource);

        Task<GroupDTO> GetGroupByIdAsync(long groupId, CancellationTokenSource cancellationTokenSource);

        Task<GroupDTO> CreateNewGroupAsync(CreateGroupDataModel createGroupDataModel, CancellationTokenSource cancellationTokenSource);

        Task<bool> GroupRequestDeclineAsync(GroupRequestDeclineDataModel groupRequestDeclineDataModel, CancellationToken cancellationToken);

        Task<GroupRequestConfirmResponse> GroupRequestConfirmAsync(GroupRequestConfirmDataModel dataModel, CancellationToken cancellationToken);

        Task<List<MemberDTO>> InviteMemberToTheGroupAsync(InviteMembersToTheGroupDataModel data, CancellationTokenSource cancellationTokenSource);
    }
}
