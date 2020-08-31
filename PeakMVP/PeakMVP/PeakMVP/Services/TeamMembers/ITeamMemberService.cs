using PeakMVP.Models.Rests.DTOs.TeamMembership;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PeakMVP.Services.TeamMembers {
    public interface ITeamMemberService {

        Task<List<TeamMember>> GetTeamMembersAsync(CancellationToken cancellationToken = default(CancellationToken), bool noRepeatings = false);

        Task<bool> AddContactInfoAsync(long teamMemberId, TeamMemberContactInfo memberContactInfo, CancellationTokenSource cancellationTokenSource);

        Task<bool> EditContactInfoAsync(long teamMemberId, IEnumerable<TeamMemberContactInfo> editedContactInfos, CancellationTokenSource cancellationTokenSource);

        Task<bool> DeleteContactInfoAsync(long teamMemberId, CancellationTokenSource cancellationTokenSource);

        Task<TeamMember> GetTeamMemberByMemberIdAsync(long profileId, CancellationToken cancellationToken = default(CancellationToken));
    }
}
