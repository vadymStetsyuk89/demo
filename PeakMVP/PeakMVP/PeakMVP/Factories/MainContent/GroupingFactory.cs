using PeakMVP.Models.Identities.Groups;
using PeakMVP.Models.Rests.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace PeakMVP.Factories.MainContent {
    public class GroupingFactory : IGroupingFactory {

        public List<Member> BuildGroupMembers(IEnumerable<MemberDTO> groupProfiles, long groupOwnerId, bool isManagementAvailable) =>
            groupProfiles
                .Select<MemberDTO, Member>((pDTO) => new Member() {
                    IsGroupOwner = pDTO.Profile.Id == groupOwnerId,
                    Profile = pDTO.Profile,
                    Status = pDTO.Status,
                    IsCanBeRemoved = pDTO.Profile.Id != groupOwnerId && isManagementAvailable
                })
                .ToList<Member>();
    }
}
