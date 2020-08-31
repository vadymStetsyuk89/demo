using PeakMVP.Models.DataItems.MainContent.Teams;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.DTOs.TeamMembership;
using PeakMVP.Models.Rests.Requests.RequestDataModels.Teams;
using PeakMVP.Models.Rests.Responses.Teams;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PeakMVP.Services.Teams {
    public interface ITeamService {
        Task<CreateTeamResponse> CreateTeamAsync(CreateTeamDataModel createTeamDataModel, CancellationToken cancellationToken = default(CancellationToken));

        Task<TeamAppointmentStatusDTO> CheckAppointmentStatusAsync(long targetTeamId, CancellationTokenSource cancellationTokenSource);

        Task<ApproveTeamRequestsResponse> ApproveTeamRequestsAsync(long requestId, long teamId, CancellationToken cancellationToken);

        Task<TeamDTO> EndPartnershipWithOrganization(long teamId, CancellationTokenSource cancellationTokenSource);

        Task<RejectTeamRequestsResponse> RejectTeamRequestsAsync(long requestId, long teamId, CancellationToken cancellationToken);

        Task<TeamDTO> ResolveFullTeamInfoByTeamIdAsync(long targetTeamId, CancellationTokenSource cancellationTokenSource);

        Task<List<TeamMember>> GetMembersByTeamIdAsync(long teamId, CancellationTokenSource cancellationTokenSource);

        Task<bool> SendTeamAppointmentRequestAsync(long targetTeamId, CancellationTokenSource cancellationTokenSource);

        Task<ProfileDTO> NixTeamAppointmentAsync(long targetTeamId, CancellationTokenSource cancellationTokenSource);

        Task<GetTeamRequestsResponse> GetTeamRequestsAsync(long id, CancellationToken cancellationToken);

        Task<bool> RemoveTeamByIdAsync(long teamId, CancellationTokenSource cancellationTokenSource);

        Task<List<TeamDTO>> GetTeamsAsync(CancellationTokenSource cancellationTokenSource);

        Task<TeamMember[]> GetFilteredMembersAsync(long teamId, TeamMemberFilters filter, CancellationTokenSource cancellationTokenSource);

        Task<bool> ResolveIsRequestToJoinTeamWasSentAsync(long targetTeamId, CancellationTokenSource cancellationTokenSource);

        Task<TeamDTO> GetTeamByIdAsync(long targetTeamId, CancellationTokenSource cancellationTokenSource);

        Task<InviteExternalMemberToTeamResponse> InviteExternalMemberToTeamAsync(ExternalMemberTeamIntive externalInvite, CancellationTokenSource cancellationTokenSource);

        Task<List<ExternalInvite>> GetExternalIvitesByTeamIdAsync(long teamId, CancellationTokenSource cancellationTokenSource);

        Task<ResendExternalMemberInviteResponse> ResendExternalMemberInviteAsync(long teamId, string externalMemberEmail, CancellationTokenSource cancellationTokenSource);
    }
}
