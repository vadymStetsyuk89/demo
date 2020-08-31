using PeakMVP.Models.Identities.Groups;
using PeakMVP.Models.Rests.DTOs;
using System.Collections.Generic;

namespace PeakMVP.Factories.MainContent {
    public interface IGroupingFactory {

        List<Member> BuildGroupMembers(IEnumerable<MemberDTO> groupProfiles, long groupOwnerId, bool isManagementAvailable);
    }
}
