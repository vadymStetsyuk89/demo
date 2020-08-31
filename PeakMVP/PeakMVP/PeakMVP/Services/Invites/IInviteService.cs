using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Responses.Invites;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PeakMVP.Services.Invites {
    public interface IInviteService {
        Task<InvitesResponse> GetInvitesAsync(CancellationToken cancellationToken = default(CancellationToken));

        Task<Tuple<List<ProfileDTO>, string>> AddMembersToTheTeamByTeamIdAsync(long targetTeamId, IEnumerable<long> targetMemberIds, CancellationTokenSource cancellationTokenSource);

        Task<TeamInviteConfirmResponse> TeamInviteConfirmAsync(long teamId, CancellationToken cancellationToken, long? profileId = null);

        Task<TeamInviteRejectResponse> TeamInviteRejectAsync(long teamId, CancellationToken cancellationToken, long? profileId = null);
    }
}
