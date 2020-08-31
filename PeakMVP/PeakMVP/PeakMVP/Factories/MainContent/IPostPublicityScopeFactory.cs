using PeakMVP.Models.Identities.Feed;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.DTOs.TeamMembership;
using System.Collections.Generic;

namespace PeakMVP.Factories.MainContent {
    public interface IPostPublicityScopeFactory {

        List<PostPublicityScope> BuildCompletedPublicityScopeList(IEnumerable<PostPublicityScope> publicityItems);

        List<PostPublicityScope> BuildCompletedPublicityScopeList(IEnumerable<PostPublicityScope> publicityItems, PostPublicityScope familyscopeItem);

        List<PostPublicityScope> BuildRawPublicityScope(IEnumerable<GroupDTO> groupsData);

        List<PostPublicityScope> BuildRawPublicityScope(IEnumerable<TeamMember> teamMemberData);

        PostPublicityScope BuildFamilyPublicityScope(FamilyDTO familyDTO);
    }
}
